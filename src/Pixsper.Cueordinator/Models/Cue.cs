using System;

namespace Pixsper.Cueordinator.Models;

public record Cue : IComparable<Cue>
{
    public CueNumber Number { get; init; }
    public string Label { get; init; } = string.Empty;

    public int CompareTo(Cue? other)
    {
        if (ReferenceEquals(this, other))
            return 0;
        if (ReferenceEquals(null, other))
            return 1;

        int numberComparison = Number.CompareTo(other.Number);
        if (numberComparison != 0)
            return numberComparison;

        return string.Compare(Label, other.Label, StringComparison.Ordinal);
    }
}