using System;
using System.Numerics;
using CSharpAlgorithms.Collections;

namespace CSharpAlgorithms;

public static class Calculator
{
    public static int NextPowerOf2(int x)
    {
        if (x < 1) return 1;
        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        return x + 1;
    }

    public static T[] Add<T>(params T[][] values) where T : struct, IAdditionOperators<T, T, T>
    {
        long length = CollectionUtils.GetLongestLength(values);
        T[] totals = new T[length];

        for (long i = 0; i < values.LongLength; i++)
        {
            T[] subArray = values[i];
            for (long y = 0; y < subArray.LongLength; y++)
            {
                totals[y] += subArray[y];
            }
        }

        return totals;
    }

    public static T ClampInclusive<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) <= 0)
            return min;

        if (value.CompareTo(max) >= 0)
            return max;

        return value;
    }
    public static void ClampInclusive<T>(T[] values, T min, T max) where T : IComparable<T>
    {
        for (long i = 0; i < values.LongLength; i++)
        {
            values[i] = ClampInclusive(values[i], min, max);
        }
    }
    public static double RateToInterval(double rate) => 1 / rate;
    public static int SecondsToMilliSeconds(double seconds) => (int)(seconds * 1_000);

    public static T Min<T>(params T[] comparables) where T : IComparisonOperators<T, T, bool>
    {
        T min = comparables[0];

        for (int i = 1; i < comparables.Length; i++)
        {
            if (comparables[i] < min)
                min = comparables[i];
        }

        return min;
    }

    public static T Max<T>(params T[] comparables) where T : IComparisonOperators<T, T, bool>
    {
        T max = comparables[0];

        for (int i = 1; i < comparables.Length; i++)
        {
            if (comparables[i] > max)
                max = comparables[i];
        }

        return max;
    }
}

