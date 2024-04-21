using System;
using System.Collections.Generic;

namespace Pixsper.Cueordinator.Models;

internal record Configuration
{
    public IReadOnlyList<IConnectionConfiguration> Connections { get; init; } = Array.Empty<IConnectionConfiguration>();

    public Guid? SourceConnectionId { get; init; }

    public IReadOnlyList<Guid> TargetConnectionIds { get; init; } = new List<Guid>();
}