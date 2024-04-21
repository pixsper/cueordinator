using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Pixsper.Cueordinator.Models;

public readonly struct CueNumberMultipart : IEquatable<CueNumberMultipart>, IComparable<CueNumberMultipart>, IComparable<CueNumberDecimal>
{
    public CueNumberMultipart()
    {
        Values = [];
    }

    public CueNumberMultipart(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than or equal to zero");

        Values = [(uint)value];
    }

    public CueNumberMultipart(uint value)
    {
        Values = [value];
    }

    public CueNumberMultipart(params int[] values)
    {
        foreach (var value in values)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(values), values, "All values must be greater than or equal to zero");
        }

        Values = [..Array.ConvertAll(values, i => (uint)i)];
    }

    public CueNumberMultipart(params uint[] values)
    {
        Values = [..values];
    }

    public ImmutableArray<uint> Values { get; }

    public bool Equals(CueNumberMultipart other)
    {
        if (Values.Length != other.Values.Length)
            return false;

        for (int i = 0; i < Values.Length; ++i)
        {
            if (Values[i] != other.Values[i])
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is CueNumberMultipart other && Equals(other);

    public override int GetHashCode() => Values.GetHashCode();

    public static bool operator ==(CueNumberMultipart left, CueNumberMultipart right) => left.Equals(right);

    public static bool operator !=(CueNumberMultipart left, CueNumberMultipart right) => !left.Equals(right);


    public int CompareTo(CueNumberMultipart other)
    {
        int i = 0;
        while (Values.Length > i && other.Values.Length > i)
        {
            uint a = Values[i];
            uint b = other.Values[i];

            if (a > b)
                return 1;
            if (a < b)
                return -1;

            ++i;
        }

        if (Values.Length < other.Values.Length)
            return -1;

        if (Values.Length > other.Values.Length)
            return 1;

        return 0;
    }

    public static bool operator <(CueNumberMultipart left, CueNumberMultipart right) => left.CompareTo(right) < 0;

    public static bool operator >(CueNumberMultipart left, CueNumberMultipart right) => left.CompareTo(right) > 0;

    public static bool operator <=(CueNumberMultipart left, CueNumberMultipart right) => left.CompareTo(right) <= 0;

    public static bool operator >=(CueNumberMultipart left, CueNumberMultipart right) => left.CompareTo(right) >= 0;


    public int CompareTo(CueNumberDecimal other)
    {
        return other.Value.CompareTo(this) switch
        {
            -1 => 1,
            0 => 0,
            1 => -1,
            _ => throw new UnreachableException()
        };
    }

    public static bool operator <(CueNumberMultipart left, CueNumberDecimal right) => left.CompareTo(right) < 0;

    public static bool operator >(CueNumberMultipart left, CueNumberDecimal right) => left.CompareTo(right) > 0;

    public static bool operator <=(CueNumberMultipart left, CueNumberDecimal right) => left.CompareTo(right) <= 0;

    public static bool operator >=(CueNumberMultipart left, CueNumberDecimal right) => left.CompareTo(right) >= 0;


    public override string ToString() => string.Join('.', Values);
}