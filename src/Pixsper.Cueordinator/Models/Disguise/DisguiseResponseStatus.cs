using System.Collections.Generic;

namespace Pixsper.Cueordinator.Models.Disguise;

internal record DisguiseResponseStatus
{
    public int Code { get; init; }

    public string Message { get; init; } = string.Empty;

    public IReadOnlyList<string> Details { get; init; } = new List<string>();
}