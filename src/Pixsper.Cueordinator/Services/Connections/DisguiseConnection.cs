using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Pixsper.Cueordinator.Models.Disguise;

namespace Pixsper.Cueordinator.Services.Connections;

internal class DisguiseConnection
{
    private readonly IHttpClientFactory _clientFactory;

    public DisguiseConnection(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<DisguiseProjectsResponse?> GetProjectsAsync(Uri serverUri, CancellationToken cancellationToken = default)
    {
        using var client = _clientFactory.CreateClient();
        var uri = new Uri(serverUri, "api/service/system/projects");

        return await client.GetFromJsonAsync<DisguiseProjectsResponse>(uri, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<DisguiseTracksResponse?> GetTracksAsync(Uri serverUri, CancellationToken cancellationToken = default)
    {
        using var client = _clientFactory.CreateClient();
        var uri = new Uri(serverUri, "api/session/transport/tracks");

        return await client.GetFromJsonAsync<DisguiseTracksResponse>(uri, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<DisguiseAnnotationsResponse?> GetAnnotationsAsync(Uri serverUri, string trackUid, CancellationToken cancellationToken = default)
    {
        using var client = _clientFactory.CreateClient();
        var uri = new Uri(serverUri, $"api/session/transport/annotations?uid={trackUid}");

        return await client.GetFromJsonAsync<DisguiseAnnotationsResponse>(uri, cancellationToken)
            .ConfigureAwait(false);
    }
}