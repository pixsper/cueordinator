using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Pixsper.Cueordinator.Models.Disguise;

namespace Pixsper.Cueordinator.Services;

public class SyncService : IAsyncDisposable
{
    private readonly IServiceProvider _serviceProvider;

    private LoopTask? _loopTask;

    public SyncService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

    }

    public async ValueTask DisposeAsync()
    {
        if (_loopTask is not null)
            await _loopTask.DisposeAsync().ConfigureAwait(false);
    }


    public async Task RunSyncAsync(CancellationToken cancellationToken)
    {
        var disguiseClient = _serviceProvider.GetRequiredService<DisguiseHttpClient>();
        var eosClient = new EosOscClient(new IPEndPoint(IPAddress.Loopback, 3037));

        var serverUri = new Uri("http://localhost:8090");

        var tracks = await disguiseClient.GetTracksAsync(serverUri, cancellationToken).ConfigureAwait(false);

        if (tracks is not null)
        {
            foreach (var track in tracks.Result)
            {
                var annotations = await disguiseClient.GetAnnotationsAsync(serverUri, track.Uid, cancellationToken)
                    .ConfigureAwait(false);

                if (annotations is not null)
                {
                    var timeline = annotations.Result.Annotations.GetByTime();

                    foreach (var annotation in timeline.Values)
                    {
                        var tag = (DisguiseTag?)annotation.FirstOrDefault(a => a is DisguiseTag { Type: DisguiseTagType.Cue });
                        var note = (DisguiseNote?)annotation.FirstOrDefault(a => a is DisguiseNote);

                        if (tag is not null && note is not null)
                        {
                            var cueNumberParts = tag.Value.Split('.');
                            if (cueNumberParts.Length == 3 
                                && byte.TryParse(cueNumberParts[0], out byte cueXx) 
                                && byte.TryParse(cueNumberParts[1], out byte cueYy)
                                && byte.TryParse(cueNumberParts[2], out byte cueZz))
                            {
                                await eosClient.ProgramDisguiseCueAsync(101, 101, cueXx, cueYy, cueZz, note.Text);
                            }
                        }
                    }
                }
            }
        }
    }
}