using System;
using System.Globalization;

namespace Pixsper.Cueordinator.Models;

public readonly struct CueNumberDecimal : IEquatable<CueNumberDecimal>, IComparable<CueNumberDecimal>, IComparable<CueNumberMultipart>
{
    public CueNumberDecimal(decimal value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than or equal to zero");

        Value = value;
    }

    public decimal Value { get; }

    public bool Equals(CueNumberDecimal other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is CueNumberDecimal other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(CueNumberDecimal left, CueNumberDecimal right) => left.Equals(right);

    public static bool operator !=(CueNumberDecimal left, CueNumberDecimal right) => !left.Equals(right);

    public int CompareTo(CueNumberDecimal other) => Value.CompareTo(other.Value);

    public static bool operator <(CueNumberDecimal left, CueNumberDecimal right) => left.CompareTo(right) < 0;

    public static bool operator >(CueNumberDecimal left, CueNumberDecimal right) => left.CompareTo(right) > 0;

    public static bool operator <=(CueNumberDecimal left, CueNumberDecimal right) => left.CompareTo(right) <= 0;

    public static bool operator >=(CueNumberDecimal left, CueNumberDecimal right) => left.CompareTo(right) >= 0;

    public int CompareTo(CueNumberMultipart other)
    {
        switch (other.Values.Length)
        {
            case 0:
                return 1;

            case 1:
                return Value.CompareTo(other.Values[0]);

            default:
            {
                var wholePart = (uint)Math.Truncate(Value);
                if (wholePart > other.Values[0])
                    return 1;
                else
                    return -1;
            }
        }
    }

    public static bool operator <(CueNumberDecimal left, CueNumberMultipart right) => left.CompareTo(right) < 0;

    public static bool operator >(CueNumberDecimal left, CueNumberMultipart right) => left.CompareTo(right) > 0;

    public static bool operator <=(CueNumberDecimal left, CueNumberMultipart right) => left.CompareTo(right) <= 0;

    public static bool operator >=(CueNumberDecimal left, CueNumberMultipart right) => left.CompareTo(right) >= 0;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public static implicit operator decimal(CueNumberDecimal cueNumber) => cueNumber.Value;
}