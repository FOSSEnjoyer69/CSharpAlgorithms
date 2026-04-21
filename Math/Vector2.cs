using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using CSharpAlgorithms.Data;
using CSharpAlgorithms.Interfaces;

namespace CSharpAlgorithms.Math;
public struct Vector2<T> : INumber<Vector2<T>>, IByteSize
                where T : struct, INumber<T>
{
    public T X;
    public T Y;

    public Vector2(T x, T y)
    {
        X = x;
        Y = y;
    }

    public Vector2(int x, int y)
    {
        X = T.CreateChecked(x);
        Y = T.CreateChecked(y);
    }

    public Vector2(BinaryReader reader)
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(int):
                X = T.CreateChecked(reader.ReadInt32());
                Y = T.CreateChecked(reader.ReadInt32());
                break;
            case Type t when t == typeof(float):
                X = T.CreateChecked(reader.ReadSingle());
                Y = T.CreateChecked(reader.ReadSingle());
                break;
            case Type t when t == typeof(double):
                X = T.CreateChecked(reader.ReadDouble());
                Y = T.CreateChecked(reader.ReadDouble());
                break;
            case Type t when t == typeof(long):
                X = T.CreateChecked(reader.ReadInt64());
                Y = T.CreateChecked(reader.ReadInt64());
                break;
            case Type t when t == typeof(short):
                X = T.CreateChecked(reader.ReadInt16());
                Y = T.CreateChecked(reader.ReadInt16());
                break;
            case Type t when t == typeof(byte):
                X = T.CreateChecked(reader.ReadByte());
                Y = T.CreateChecked(reader.ReadByte());
                break;
            case Type t when t == typeof(bool):
                X = T.CreateChecked(reader.ReadBoolean() ? 1 : 0);
                Y = T.CreateChecked(reader.ReadBoolean() ? 1 : 0);
                break;
            default:
                return;
        }
    }

    public static Vector2<T> One => new(1, 1);
    public static int Radix => throw new NotImplementedException();

    public static Vector2<T> Zero => new(0, 0);

    public static Vector2<T> AdditiveIdentity => throw new NotImplementedException();

    public static Vector2<T> MultiplicativeIdentity => throw new NotImplementedException();

    public static Vector2<T> Abs(Vector2<T> value)
    {
        T x = T.Abs(value.X);
        T y = T.Abs(value.Y);
        return new Vector2<T>(x, y);
    }

    public static bool IsCanonical(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsComplexNumber(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsEvenInteger(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsFinite(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsImaginaryNumber(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInfinity(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInteger(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNaN(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegative(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegativeInfinity(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNormal(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsOddInteger(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsPositive(Vector2<T> value) => value.X > T.Zero && value.Y > T.Zero;

    public static bool IsPositiveInfinity(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsRealNumber(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsSubnormal(Vector2<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsZero(Vector2<T> value) => value.X == T.Zero && value.Y == T.Zero;

    public static Vector2<T> MaxMagnitude(Vector2<T> x, Vector2<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector2<T> MaxMagnitudeNumber(Vector2<T> x, Vector2<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector2<T> MinMagnitude(Vector2<T> x, Vector2<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector2<T> MinMagnitudeNumber(Vector2<T> x, Vector2<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector2<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector2<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector2<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector2<T> Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out Vector2<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out Vector2<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out Vector2<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(Vector2<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(Vector2<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(Vector2<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Vector2<T> result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Vector2<T> result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Vector2<T> result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Vector2<T> result)
    {
        throw new NotImplementedException();
    }

    public void BinarySerialize(BinaryWriter writer)
    {
        if (typeof(T) == typeof(int))
        {
            writer.Write(Convert.ToInt32(X));
            writer.Write(Convert.ToInt32(Y));
        }
        else if (typeof(T) == typeof(float))
        {
            writer.Write(Convert.ToSingle(X));
            writer.Write(Convert.ToSingle(Y));
        }
        else if (typeof(T) == typeof(double))
        {
            writer.Write(Convert.ToDouble(X));
            writer.Write(Convert.ToDouble(Y));
        }
        else if (typeof(T) == typeof(long))
        {
            writer.Write(Convert.ToInt64(X));
            writer.Write(Convert.ToInt64(Y));
        }
        else if (typeof(T) == typeof(short))
        {
            writer.Write(Convert.ToInt16(X));
            writer.Write(Convert.ToInt16(Y));
        }
        else if (typeof(T) == typeof(byte))
        {
            writer.Write(Convert.ToByte(X));
            writer.Write(Convert.ToByte(Y));
        }
        else if (typeof(T) == typeof(bool))
        {
            writer.Write(Convert.ToBoolean(X));
            writer.Write(Convert.ToBoolean(Y));
        }
        else
        {
            throw new NotSupportedException($"Binary serialization for type {typeof(T)} is not supported.");
        }
    }

    public int CompareTo(object? obj)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(Vector2<T> other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Vector2<T> other) => X.Equals(other.X) && Y.Equals(other.Y);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return $"[CSharpAlgorithms.Math.Vector2<{typeof(T).Name}>: X={X.ToString(format, formatProvider)}, Y={Y.ToString(format, formatProvider)}]";
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public readonly uint GetByteSize() => (uint)(Marshal.SizeOf<T>() * 2);
    public readonly void WriteBinaryData(BinaryWriter writer)
    {
        writer.TryWriteGeneric(X);
        writer.TryWriteGeneric(Y);
    }

    public static Vector2<T> operator +(Vector2<T> value) => value;
    public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right)
    {
        T x = left.X + right.X;
        T y = left.Y + right.Y;
        return new Vector2<T>(x, y);
    }

    public static Vector2<T> operator -(Vector2<T> value) => new(-value.X, -value.Y);
    public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right)
    {
        T x = left.X - right.X;
        T y = left.Y - right.Y;
        return new Vector2<T>(x, y);
    }

    public static Vector2<T> operator ++(Vector2<T> value)
    {
        T x = value.X + T.One;
        T y = value.Y + T.One;
        return new Vector2<T>(x, y);
    }

    public static Vector2<T> operator --(Vector2<T> value)
    {
        T x = value.X - T.One;
        T y = value.Y - T.One;
        return new Vector2<T>(x, y);
    }

    public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right)
    {
        T x = left.X * right.X;
        T y = left.Y * right.Y;
        return new Vector2<T>(x, y);
    }

    public static Vector2<T> operator /(Vector2<T> left, Vector2<T> right)
    {
        T x = left.X / right.X;
        T y = left.Y / right.Y;
        return new Vector2<T>(x, y);
    }

    public static Vector2<T> operator %(Vector2<T> left, Vector2<T> right)
    {
        T x = left.X % right.X;
        T y = left.Y % right.Y;
        return new Vector2<T>(x, y);
    }

    public static bool operator ==(Vector2<T> left, Vector2<T> right) => left.Equals(right);
    public static bool operator !=(Vector2<T> left, Vector2<T> right) => !left.Equals(right);

    public static bool operator <(Vector2<T> left, Vector2<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y;
        T rightMagnitude = right.X * right.X + right.Y * right.Y;
        return leftMagnitude < rightMagnitude;
    }

    public static bool operator >(Vector2<T> left, Vector2<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y;
        T rightMagnitude = right.X * right.X + right.Y * right.Y;
        return leftMagnitude > rightMagnitude;
    }

    public static bool operator <=(Vector2<T> left, Vector2<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y;
        T rightMagnitude = right.X * right.X + right.Y * right.Y;
        return leftMagnitude <= rightMagnitude;
    }

    public static bool operator >=(Vector2<T> left, Vector2<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y;
        T rightMagnitude = right.X * right.X + right.Y * right.Y;
        return leftMagnitude >= rightMagnitude;
    }
}