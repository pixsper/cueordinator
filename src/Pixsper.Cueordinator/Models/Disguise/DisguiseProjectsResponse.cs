using Pixsper.Cueordinator.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pixsper.Cueordinator.Models.Disguise;

internal record DisguiseProjectsResponse
{
    public DisguiseResponseStatus Status { get; init; } = new();

    public IReadOnlyList<DisguiseHostProjectsList> Result { get; init; } = [];
}

internal record DisguiseHostProjectsList
{
    public string Hostname { get; init; } = string.Empty;

    public string LastProject { get; init; } = string.Empty;

    public IReadOnlyList<DisguiseProject> Projects { get; init; } = [];
}


internal record DisguiseProject
{
    public string Path { get; init; } = string.Empty;

    [JsonConverter(typeof(DisguiseDateTimeJsonConverter))]
    public DateTime LastModified { get; init; }

    public DisguiseSoftwareVersion Version { get; init; } = new();
}

internal record DisguiseSoftwareVersion
{
    public int Major { get; init; }
    public int Minor { get; init; }
    public int Hotfix { get; init; }
    public int Revision { get; init; }
}