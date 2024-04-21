using System;
using DynamicData;
using Pixsper.Cueordinator.Models;

namespace Pixsper.Cueordinator.Services;

public interface IConfigurationService
{
    IObservableCache<IConnectionConfiguration, Guid> Connections { get; }
    IObservableValue<Guid?> SourceConnectionId { get; }
    IObservableList<Guid> TargetConnectionIds { get; }

    void CreateOrUpdateConnection(IConnectionConfiguration connection);
    void DeleteConnection(IConnectionConfiguration connection);
    void DeleteConnection(Guid id);

    public void SetSourceConnectionId(Guid? id);
    public void CreateTargetConnection(Guid id);
    public void DeleteTargetConnection(Guid id);
}