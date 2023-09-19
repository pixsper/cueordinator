using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pixsper.Cueordinator.Models.Disguise;

internal record DisguiseTracksResponse
{
    public DisguiseResponseStatus Status { get; init; } = new();

    public IReadOnlyList<DisguiseTrack> Result { get; init; } = new List<DisguiseTrack>();
}


internal record DisguiseTrack
{
    public string Uid { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Length { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DisguiseCrossfadeMode Crossfade { get; init; } = DisguiseCrossfadeMode.Off;
}