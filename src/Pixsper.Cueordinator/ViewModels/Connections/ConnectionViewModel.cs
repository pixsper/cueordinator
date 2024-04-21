using System;
using Pixsper.Cueordinator.Models;
using ReactiveUI;

namespace Pixsper.Cueordinator.ViewModels.Connections;

public abstract class ConnectionViewModel : ReactiveObject, IDisposable
{
    public abstract void Dispose();

    public abstract ConnectionKind Kind { get; }

    public Guid Id { get; }
}