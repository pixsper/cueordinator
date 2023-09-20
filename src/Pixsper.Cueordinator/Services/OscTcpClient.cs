using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using OscCore;

namespace Pixsper.Cueordinator.Services;

internal class OscTcpClient : IAsyncDisposable
{
    private const byte SlipEnd = 192;
    private const byte SlipEsc = 219;

    private const int BufferLength = 4096;
    private const int ConnectionAttemptIntervalMs = 1000;

    private readonly LoopTask _loopTask;
    private readonly IPEndPoint _remoteEndpoint;
    private readonly TcpClient _tcpClient;

    private NetworkStream? _stream;

    private readonly AsyncLock _outputLock = new();
    private readonly byte[] _outputBuffer = new byte[BufferLength];
        
        
    public OscTcpClient(IPEndPoint remoteEndPoint, IPEndPoint localEndpoint)
    {
        _remoteEndpoint = remoteEndPoint;
        _tcpClient = new TcpClient(localEndpoint);


        _loopTask = new LoopTask(loopImpl);
    }

    public async ValueTask DisposeAsync()
    {
        using var _ = await _outputLock.LockAsync().ConfigureAwait(false);
        await _loopTask.DisposeAsync().ConfigureAwait(false);
    }

    public async Task<bool> SendAsync(OscPacket packet)
    {
        var stream = _stream;

        if (stream is null)
            return false;

        using var _ = await _outputLock.LockAsync().ConfigureAwait(false);

        var bytes = packet.ToByteArray();
        if (bytes is null)
            throw new ArgumentException("Packet is invalid", nameof(packet));

        int p = 0;
        _outputBuffer[p++] = SlipEnd;
        for (int i = 0; i < bytes.Length; i++)
        {
            var b = bytes[i];
            switch (b)
            {
                case SlipEnd:
                    _outputBuffer[p++] = SlipEsc;
                    _outputBuffer[p++] = 220;
                    break;
                case SlipEsc:
                    _outputBuffer[p++] = SlipEsc;
                    _outputBuffer[p++] = 221;
                    break;
                default:
                    _outputBuffer[p++] = b;
                    break;
            }
        }

        _outputBuffer[p++] = SlipEnd;

        await stream.WriteAsync(_outputBuffer, 0, p).ConfigureAwait(false);
        return true;
    }


    private async Task loopImpl(CancellationToken cancellationToken)
    {
        try
        {
            await _tcpClient.ConnectAsync(_remoteEndpoint, cancellationToken).ConfigureAwait(false);

            _stream = _tcpClient.GetStream();
            var buffer = new byte[BufferLength];
            var packetBuffer = new byte[BufferLength];
            int p = 0;

            while (_tcpClient.Connected)
            {
                int bytesReceived = await _stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                for (int i = 0; i < bytesReceived; i++)
                {
                    byte b = buffer[i];
                    switch (b)
                    {
                        case SlipEnd:
                            if (p > 0)
                            {
                                var packet = OscPacket.Read(packetBuffer, 0, p);
                                Debug.WriteLine(packet.ToString());
                            }
                            p = 0;
                            break;

                        case SlipEsc:
                            b = buffer[++i];
                            switch (b)
                            {
                                case 220:
                                    packetBuffer[p++] = SlipEnd;
                                    break;
                                case 221:
                                    packetBuffer[p++] = SlipEsc;
                                    break;
                            }
                            break;
                        default:
                            packetBuffer[p++] = b;
                            break;
                    }
                }
            }
        }
        catch (SocketException)
        {
            
        }

        await Task.Delay(ConnectionAttemptIntervalMs, cancellationToken).ConfigureAwait(false);
    }
}