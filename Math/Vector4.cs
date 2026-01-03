using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace CSharpAlgorithms.Math;

public struct Vector4<T> : INumber<Vector4<T>> where T : struct, INumber<T>
{
    public T X;
    public T Y;
    public T Z;
    public T W;

    public Vector4(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Vector4(int x, int y, int z, int w)
    {
        X = T.CreateChecked(x);
        Y = T.CreateChecked(y);
        Z = T.CreateChecked(z);
        W = T.CreateChecked(w);
    }

    public static Vector4<T> One  => new Vector4<T>(1, 1, 1, 1);
    public static Vector4<T> Zero => new Vector4<T>(0, 0, 0, 0);

    public static int Radix => throw new NotImplementedException();

    public static Vector4<T> AdditiveIdentity => throw new NotImplementedException();

    public static Vector4<T> MultiplicativeIdentity => throw new NotImplementedException();

    public static Vector4<T> Abs(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsCanonical(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsComplexNumber(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsEvenInteger(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsFinite(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsImaginaryNumber(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInfinity(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInteger(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNaN(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegative(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegativeInfinity(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNormal(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsOddInteger(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsPositive(Vector4<T> value)
        => value.X > T.Zero && value.Y > T.Zero && value.Z > T.Zero && value.W > T.Zero;

    public static bool IsPositiveInfinity(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsRealNumber(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsSubnormal(Vector4<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsZero(Vector4<T> value)
        => value.X == T.Zero && value.Y == T.Zero && value.Z == T.Zero && value.W == T.Zero;

    public static Vector4<T> MaxMagnitude(Vector4<T> x, Vector4<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector4<T> MaxMagnitudeNumber(Vector4<T> x, Vector4<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector4<T> MinMagnitude(Vector4<T> x, Vector4<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector4<T> MinMagnitudeNumber(Vector4<T> x, Vector4<T> y)
    {
        throw new NotImplementedException();
    }

    public static Vector4<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector4<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector4<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector4<T> Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromChecked<TOther>(
        TOther value,
        [MaybeNullWhen(false)] out Vector4<T> result
    ) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(
        TOther value,
        [MaybeNullWhen(false)] out Vector4<T> result
    ) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(
        TOther value,
        [MaybeNullWhen(false)] out Vector4<T> result
    ) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(
        Vector4<T> value,
        [MaybeNullWhen(false)] out TOther result
    ) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(
        Vector4<T> value,
        [MaybeNullWhen(false)] out TOther result
    ) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(
        Vector4<T> value,
        [MaybeNullWhen(false)] out TOther result
    ) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector4<T> result
    )
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        NumberStyles style,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector4<T> result
    )
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector4<T> result
    )
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Vector4<T> result
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
            writer.Write(Convert.ToInt32(W));
        }
        else if (typeof(T) == typeof(float))
        {
            writer.Write(Convert.ToSingle(X));
            writer.Write(Convert.ToSingle(Y));
            writer.Write(Convert.ToSingle(Z));
            writer.Write(Convert.ToSingle(W));
        }
        else if (typeof(T) == typeof(double))
        {
            writer.Write(Convert.ToDouble(X));
            writer.Write(Convert.ToDouble(Y));
            writer.Write(Convert.ToDouble(Z));
            writer.Write(Convert.ToDouble(W));
        }
        else if (typeof(T) == typeof(long))
        {
            writer.Write(Convert.ToInt64(X));
            writer.Write(Convert.ToInt64(Y));
            writer.Write(Convert.ToInt64(Z));
            writer.Write(Convert.ToInt64(W));
        }
        else if (typeof(T) == typeof(short))
        {
            writer.Write(Convert.ToInt16(X));
            writer.Write(Convert.ToInt16(Y));
            writer.Write(Convert.ToInt16(Z));
            writer.Write(Convert.ToInt16(W));
        }
        else if (typeof(T) == typeof(byte))
        {
            writer.Write(Convert.ToByte(X));
            writer.Write(Convert.ToByte(Y));
            writer.Write(Convert.ToByte(Z));
            writer.Write(Convert.ToByte(W));
        }
        else if (typeof(T) == typeof(bool))
        {
            writer.Write(Convert.ToBoolean(X));
            writer.Write(Convert.ToBoolean(Y));
            writer.Write(Convert.ToBoolean(Z));
            writer.Write(Convert.ToBoolean(W));
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

    public int CompareTo(Vector4<T> other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Vector4<T> other)
        => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    public override bool Equals(object? obj)
        => obj is Vector4<T> v && Equals(v);

    public override int GetHashCode()
        => HashCode.Combine(X, Y, Z, W);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return
            $"[CSharpAlgorithms.Math.Vector4<{typeof(T).Name}>: " +
            $"X={X.ToString(format, formatProvider)}, " +
            $"Y={Y.ToString(format, formatProvider)}, " +
            $"Z={Z.ToString(format, formatProvider)}, " +
            $"W={W.ToString(format, formatProvider)}]";
    }

    public override string ToString()
        => ToString(null, CultureInfo.InvariantCulture);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider
    )
    {
        throw new NotImplementedException();
    }

    // Unary
    public static Vector4<T> operator +(Vector4<T> value) => value;

    public static Vector4<T> operator -(Vector4<T> value)
        => new Vector4<T>(-value.X, -value.Y, -value.Z, -value.W);

    // Add / Sub
    public static Vector4<T> operator +(Vector4<T> left, Vector4<T> right)
        => new Vector4<T>(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);

    public static Vector4<T> operator -(Vector4<T> left, Vector4<T> right)
        => new Vector4<T>(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

    // ++ / --
    public static Vector4<T> operator ++(Vector4<T> value)
        => new Vector4<T>(value.X + T.One, value.Y + T.One, value.Z + T.One, value.W + T.One);

    public static Vector4<T> operator --(Vector4<T> value)
        => new Vector4<T>(value.X - T.One, value.Y - T.One, value.Z - T.One, value.W - T.One);

    // Component-wise * / %
    public static Vector4<T> operator *(Vector4<T> left, Vector4<T> right)
        => new Vector4<T>(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);

    public static Vector4<T> operator /(Vector4<T> left, Vector4<T> right)
        => new Vector4<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);

    public static Vector4<T> operator %(Vector4<T> left, Vector4<T> right)
        => new Vector4<T>(left.X % right.X, left.Y % right.Y, left.Z % right.Z, left.W % right.W);

    // Equality
    public static bool operator ==(Vector4<T> left, Vector4<T> right) => left.Equals(right);
    public static bool operator !=(Vector4<T> left, Vector4<T> right) => !left.Equals(right);

    // Magnitude-based comparisons
    public static bool operator <(Vector4<T> left, Vector4<T> right)
    {
        T leftMagnitude  = left.X * left.X + left.Y * left.Y + left.Z * left.Z + left.W * left.W;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z + right.W * right.W;
        return leftMagnitude < rightMagnitude;
    }

    public static bool operator >(Vector4<T> left, Vector4<T> right)
    {
        T leftMagnitude  = left.X * left.X + left.Y * left.Y + left.Z * left.Z + left.W * left.W;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z + right.W * right.W;
        return leftMagnitude > rightMagnitude;
    }

    public static bool operator <=(Vector4<T> left, Vector4<T> right)
    {
        T leftMagnitude  = left.X * left.X + left.Y * left.Y + left.Z * left.Z + left.W * left.W;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z + right.W * right.W;
        return leftMagnitude <= rightMagnitude;
    }

    public static bool operator >=(Vector4<T> left, Vector4<T> right)
    {
        T leftMagnitude  = left.X * left.X + left.Y * left.Y + left.Z * left.Z + left.W * left.W;
        T rightMagnitude = right.X * right.X + right.Y * right.Y + right.Z * right.Z + right.W * right.W;
        return leftMagnitude >= rightMagnitude;
    }
}
