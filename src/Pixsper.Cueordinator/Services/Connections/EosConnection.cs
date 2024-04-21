using System;
using System.Net;
using System.Threading.Tasks;
using OscCore;
using Pixsper.Cueordinator.Services.Net;

namespace Pixsper.Cueordinator.Services.Connections;

internal class EosConnection : IAsyncDisposable
{
    private readonly OscTcpClient _client;

    public EosConnection(IPEndPoint remoteEndpoint)
    {
        _client = new OscTcpClient(remoteEndpoint, new IPEndPoint(IPAddress.Any, 0));
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync().ConfigureAwait(false);
    }

    public async Task ProgramDisguiseCueAsync(int disguiseChannel, int eosCueList, byte cueXx, byte cueYy, byte cueZz, string label, bool isFirst = false)
    {
        await _client.SendAsync(new OscMessage("/eos/user", 0));

        if (isFirst)
        {
            await _client.SendAsync(new OscMessage("/eos/newcmd", $"Chan {disguiseChannel} @ Full#"));
            await _client.SendAsync(new OscMessage("/eos/newcmd", $"Chan {disguiseChannel} La_Cmnd 26#"));
            await _client.SendAsync(new OscMessage("/eos/newcmd", $"Chan {disguiseChannel} Play_Mode 26#"));
        }

        await _client.SendAsync(new OscMessage("/eos/newcmd", $"Chan {disguiseChannel} _Cue {cueXx:D3}#"));
        await _client.SendAsync(new OscMessage("/eos/newcmd", $"Chan {disguiseChannel} Cue_2 {cueYy:D3}#"));
        await _client.SendAsync(new OscMessage("/eos/newcmd", $"Chan {disguiseChannel} Cue_3 {cueZz:D3}#"));

        decimal cueNumber = (cueXx * 100m) + cueYy + (cueZz * 0.01m);
        await _client.SendAsync(new OscMessage("/eos/newcmd", $"Chan {disguiseChannel} Record Cue {eosCueList} / {cueNumber:F2} Time 0 Label {label}#"));
    }
}