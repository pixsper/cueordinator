using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Pixsper.Cueordinator.Services;

internal class ConnectionsService : IConnectionsService, IAsyncDisposable
{
    private readonly ILogger _log;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationService _configurationService;

    public ConnectionsService(ILogger<ConnectionsService> log, IServiceProvider serviceProvider, IConfigurationService configurationService)
    {
        _log = log;
        _serviceProvider = serviceProvider;
        _configurationService = configurationService;
    }


    public async ValueTask DisposeAsync()
    {

    }
}