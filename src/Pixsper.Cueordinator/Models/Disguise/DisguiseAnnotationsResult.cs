using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Pixsper.Cueordinator.Models.Disguise;

internal record DisguiseAnnotationsResponse
{
    public DisguiseResponseStatus Status { get; init; } = new();

    public DisguiseAnnotationsResult Result { get; init; } = new();
}

internal record DisguiseAnnotationsResult
{
    public string Uid { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DisguiseAnnotations Annotations { get; init; } = new();
}

internal record DisguiseAnnotations
{
    public IReadOnlyList<DisguiseNote> Notes { get; init; } = new List<DisguiseNote>();
    public IReadOnlyList<DisguiseTag> Tags { get; init; } = new List<DisguiseTag>();
    public IReadOnlyList<DisguiseSection> Sections { get; init; } = new List<DisguiseSection>();

    public IReadOnlyDictionary<int, IReadOnlyList<IDisguiseAnnotation>> GetByTime()
    {
        var items = new List<IDisguiseAnnotation>();
        items.AddRange(Notes);
        items.AddRange(Tags);
        items.AddRange(Sections);

        return items.GroupBy(i => i.Time)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<IDisguiseAnnotation>)g.ToList());
    }
}

internal interface IDisguiseAnnotation
{
    int Time { get; }
}

internal record DisguiseNote : IDisguiseAnnotation
{
    public int Time { get; init; }
    public string Text { get; init; } = string.Empty;
}

internal record DisguiseTag : IDisguiseAnnotation
{
    public int Time { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DisguiseTagType Type { get; init; }

    public string Value { get; init; } = string.Empty;
}

internal record DisguiseSection : IDisguiseAnnotation
{
    public int Time { get; init; }
    public string Index { get; init; } = string.Empty;
}