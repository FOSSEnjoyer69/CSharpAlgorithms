namespace CSharpAlgorithms.Math;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Numerics;
using CSharpAlgorithms.Interfaces;

public struct Vector3<T> : INumber<Vector3<T>> , IByteSize, IEquatable<Vector3<T>> where T : struct, INumber<T>
{
    public T X;
    public T Y;
    public T Z;

    public readonly T Magnitude => T.CreateChecked(MathF.Sqrt((float)(dynamic)(X * X + Y * Y + Z * Z)));

    public Vector3(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3(int x, int y, int z)
    {
        X = T.CreateChecked(x);
        Y = T.CreateChecked(y);
        Z = T.CreateChecked(z);
    }

    public Vector3(BinaryReader reader)
    {
        X = T.CreateChecked(reader.ReadSingle());
        Y = T.CreateChecked(reader.ReadSingle());
        Z = T.CreateChecked(reader.ReadSingle());
    }

    public static Vector3<T> One => new Vector3<T>(T.One, T.One, T.One);
    public static Vector3<T> Zero => new Vector3<T>(T.Zero, T.Zero, T.Zero);

    public static int Radix => throw new NotImplementedException();
    public static Vector3<T> AdditiveIdentity => throw new NotImplementedException();
    public static Vector3<T> MultiplicativeIdentity => throw new NotImplementedException();

    public static Vector3<T> Abs(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsCanonical(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsComplexNumber(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsEvenInteger(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsFinite(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsImaginaryNumber(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInfinity(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInteger(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNaN(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegative(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegativeInfinity(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNormal(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsOddInteger(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsPositive(Vector3<T> value)
        => value.X > T.Zero && value.Y > T.Zero && value.Z > T.Zero;

    public static bool IsPositiveInfinity(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsRealNumber(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsSubnormal(Vector3<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsZero(Vector3<T> value)
        => value.X == T.Zero && value.Y == T.Zero && value.Z == T.Zero;

    public static Vector3<T> MaxMagnitude(Vector3<T> x, Vector3<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector3<T> MaxMagnitudeNumber(Vector3<T> x, Vector3<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector3<T> MinMagnitude(Vector3<T> x, Vector3<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector3<T> MinMagnitudeNumber(Vector3<T> x, Vector3<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector3<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector3<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector3<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector3<T> Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromChecked<TOther>(
        TOther value,
        [MaybeNullWhen(false)] out Vector3<T> result
    )
        where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(
        TOther value,
        [MaybeNullWhen(false)] out Vector3<T> result
    )
        where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(
        TOther value,
        [MaybeNullWhen(false)] out Vector3<T> result
    )
        where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(
        Vector3<T> value,
        [MaybeNullWhen(false)] out TOther result
    )
        where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(
        Vector3<T> value,
        [MaybeNullWhen(false)] out TOther result
    )
        where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(
        Vector3<T> value,
        [MaybeNullWhen(false)] out TOther result
    )
        where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector3<T> result
    )
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        NumberStyles style,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector3<T> result
    )
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector3<T> result
    )
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector3<T> result
    )
    {
        throw new NotImplementedException();
    }

    public void BinarySerialize(BinaryWriter writer)
    {
        if (typeof(T) == typeof(int))
        {
            writer.Write(Convert.ToInt32(X));
            writer.Write(Convert.ToInt32(Y));
            writer.Write(Convert.ToInt32(Z));
        }
        else if (typeof(T) == typeof(float))
        {
            writer.Write(Convert.ToSingle(X));
            writer.Write(Convert.ToSingle(Y));
            writer.Write(Convert.ToSingle(Z));
        }
        else if (typeof(T) == typeof(double))
        {
            writer.Write(Convert.ToDouble(X));
            writer.Write(Convert.ToDouble(Y));
            writer.Write(Convert.ToDouble(Z));
        }
        else if (typeof(T) == typeof(long))
        {
            writer.Write(Convert.ToInt64(X));
            writer.Write(Convert.ToInt64(Y));
            writer.Write(Convert.ToInt64(Z));
        }
        else if (typeof(T) == typeof(short))
        {
            writer.Write(Convert.ToInt16(X));
            writer.Write(Convert.ToInt16(Y));
            writer.Write(Convert.ToInt16(Z));
        }
        else if (typeof(T) == typeof(byte))
        {
            writer.Write(Convert.ToByte(X));
            writer.Write(Convert.ToByte(Y));
            writer.Write(Convert.ToByte(Z));
        }
        else if (typeof(T) == typeof(bool))
        {
            writer.Write(Convert.ToBoolean(X));
            writer.Write(Convert.ToBoolean(Y));
            writer.Write(Convert.ToBoolean(Z));
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

    public int CompareTo(Vector3<T> other)
    {
        throw new NotImplementedException();
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return $"[CSharpAlgorithms.Math.Vector3<{typeof(T).Name}>: " +
               $"X={X.ToString(format, formatProvider)}, " +
               $"Y={Y.ToString(format, formatProvider)}, " +
               $"Z={Z.ToString(format, formatProvider)}]";
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider
    )
    {
        throw new NotImplementedException();
    }

    public readonly unsafe uint GetByteSize()
    {
        #pragma warning disable
        return (uint)(sizeof(T) * 3);
        #pragma warning restore
    }

    public static Vector3<T> operator +(Vector3<T> value) => value;
    public static Vector3<T> operator -(Vector3<T> value) => new(-value.X, -value.Y, -value.Z);

    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Vector3<T> operator ++(Vector3<T> value) => new(value.X + T.One, value.Y + T.One, value.Z + T.One);
    public static Vector3<T> operator --(Vector3<T> value) => new(value.X - T.One, value.Y - T.One, value.Z - T.One);

    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    public static Vector3<T> operator *(Vector3<T> left, T scalar) => new(left.X * scalar, left.Y * scalar, left.Z * scalar);
    public static Vector3<T> operator *(Vector3<T> left, float scalar)
    {
        return new Vector3<T>(
            T.CreateChecked(left.X * T.CreateChecked(scalar)),
            T.CreateChecked(left.Y * T.CreateChecked(scalar)),
            T.CreateChecked(left.Z * T.CreateChecked(scalar))
        );
    }
    
    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) => new Vector3<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    public static Vector3<T> operator %(Vector3<T> left, Vector3<T> right) => new Vector3<T>(left.X % right.X, left.Y % right.Y, left.Z % right.Z);

    // Equality
    public static bool operator ==(Vector3<T> left, Vector3<T> right) => left.Equals(right);
    public static bool operator !=(Vector3<T> left, Vector3<T> right) => !left.Equals(right);

    // Magnitude-based comparisons
    public static bool operator <(Vector3<T> left, Vector3<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y + left.Z * left.Z;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z;
        return leftMagnitude < rightMagnitude;
    }

    public static bool operator >(Vector3<T> left, Vector3<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y + left.Z * left.Z;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z;
        return leftMagnitude > rightMagnitude;
    }

    public static bool operator <=(Vector3<T> left, Vector3<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y + left.Z * left.Z;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z;
        return leftMagnitude <= rightMagnitude;
    }

    public static bool operator >=(Vector3<T> left, Vector3<T> right)
    {
        T leftMagnitude = left.X * left.X + left.Y * left.Y + left.Z * left.Z;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z;
        return leftMagnitude >= rightMagnitude;
    }

    public bool Equals(Vector3<T> other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    public override bool Equals(object? obj)
    {
        return obj is null || obj is Vector3<T> && Equals((Vector3<T>)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}
