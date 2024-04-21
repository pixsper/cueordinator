using System;
using System.Diagnostics;

namespace Pixsper.Cueordinator.Models;

public readonly struct CueNumber : IEquatable<CueNumber>, IComparable<CueNumber>
{
    private readonly CueNumberKind _kind;
    private readonly CueNumberDecimal _decimal;
    private readonly CueNumberMultipart _multipart;

    public CueNumber(decimal value)
    {
        _kind = CueNumberKind.Decimal;
        _decimal = new CueNumberDecimal(value);
    }

    public CueNumber(CueNumberMultipart value)
    {
        _kind = CueNumberKind.Multipart;
        _multipart = value;
    }


    public CueNumberDecimal? Decimal => _kind is CueNumberKind.Decimal ? _decimal : null;

    public CueNumberMultipart? Multipart => _kind is CueNumberKind.Multipart ? _multipart : null;

    public CueNumberKind Kind => _kind;


    public bool Equals(CueNumber other)
    {
        if (_kind != other._kind)
            return false;

        return _kind switch
        {
            CueNumberKind.Decimal => _decimal.Equals(other._decimal),
            CueNumberKind.Multipart => _multipart.Equals(other._multipart),
            _ => throw new UnreachableException()
        };
    }

    public override bool Equals(object? obj) => obj is CueNumber other && Equals(other);

    public override int GetHashCode()
    {
        return _kind switch
        {
            CueNumberKind.Decimal => _decimal.GetHashCode(),
            CueNumberKind.Multipart => _multipart.GetHashCode(),
            _ => throw new UnreachableException()
        };
    }

    public static bool operator ==(CueNumber left, CueNumber right) => left.Equals(right);

    public static bool operator !=(CueNumber left, CueNumber right) => !left.Equals(right);

    public int CompareTo(CueNumber other)
    {
        return (_kind, other._kind) switch
        {
            (CueNumberKind.Decimal, CueNumberKind.Decimal) => _decimal.CompareTo(other._decimal),
            (CueNumberKind.Multipart, CueNumberKind.Multipart) => _multipart.CompareTo(other._multipart),
            (CueNumberKind.Decimal, CueNumberKind.Multipart) => _decimal.CompareTo(other._multipart),
            (CueNumberKind.Multipart, CueNumberKind.Decimal) => _multipart.CompareTo(other._decimal),
            _ => throw new UnreachableException()
        };
    }

    public static bool operator <(CueNumber left, CueNumber right) => left.CompareTo(right) < 0;

    public static bool operator >(CueNumber left, CueNumber right) => left.CompareTo(right) > 0;

    public static bool operator <=(CueNumber left, CueNumber right) => left.CompareTo(right) <= 0;

    public static bool operator >=(CueNumber left, CueNumber right) => left.CompareTo(right) >= 0;
}