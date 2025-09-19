using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Sum of values of a data set divided by number of values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns>Mean Averge</returns>
    public static T MeanAverage<T>(params T[] values) where T : struct, IAdditionOperators<T, T, T>, IDivisionOperators<T, int, T>
    {
        T total = default;

        for (long i = 0; i < values.LongLength; i++)
            total += values[i];

        return total / values.Length;
    }

    /// <summary>
    /// Most frequent value in a data set 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    public static T ModeAverage<T>(params T[] values) where T : notnull
    {
        Dictionary<T, int> counts = new Dictionary<T, int>();

        for (long i = 0; i < values.LongLength; i++)
        {
            if (counts.ContainsKey(values[i]))
                counts[values[i]]++;
            else
                counts[values[i]] = 1;
        }

        T mode = default;
        int maxCount = 0;

        foreach (var pair in counts)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                mode = pair.Key;
            }
        }

        return mode;
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
            values[i] = ClampInclusive(values[i], min, max);
    }
    public static double RateToInterval(double rate) => 1 / rate;

    public static T Min<T>(params T[] comparables) where T : IComparisonOperators<T, T, bool>
    {
        T min = comparables[0];

        for (int i = 1; i < comparables.Length; i++)
            if (comparables[i] < min)
                min = comparables[i];

        return min;
    }

    public static T Max<T>(params T[] comparables) where T : IComparisonOperators<T, T, bool>
    {
        T max = comparables[0];

        for (int i = 1; i < comparables.Length; i++)
            if (comparables[i] > max)
                max = comparables[i];

        return max;
    }
}

