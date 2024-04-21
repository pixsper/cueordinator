using System;
using Pixsper.Cueordinator.Models;

namespace Pixsper.Cueordinator.Services.Connections;

public enum ConnectionStatus
{
    Disabled,
    NeverConnected,
    Disconnected,
    Connected
}

public interface IConnection : IAsyncDisposable
{
    IObservableValue<ConnectionStatus> Status { get; }
}