#pragma warning disable

using System;
using System.Collections.Generic;
using System.Numerics;
using CSharpAlgorithms.Collection;

namespace CSharpAlgorithms.Math;

public static class Calculator
{
    public static double RateToInterval(double rate) => 1 / rate;

    public static T ClampInclusive<T>(T value) where T: INumber<T> => ClampInclusive(value, T.Zero, T.One);
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

    public static T Min<T>(T a, T b) where T : IComparisonOperators<T, T, bool>
    {
        if (a < b)
            return a;
        else
            return b;
    }
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

    /// <summary>
    /// Gets the sum of a array of values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    public static T Sum<T>(params T[] values) where T : struct, INumber<T>
    {
        T total = T.Zero;

        for (int i = 0; i < values.Length; i++)
            total += values[i];

        return total;
    }
    public static T Sum<T>(IEnumerable<T> values) where T : struct, INumber<T>
    {
        T total = T.Zero;

        foreach (T value in values)
            total += value;

        return total;
    }
    public static T[] Add<T>(params T[][] values) where T : struct, IAdditionOperators<T, T, T>
    {
        long length = ArrayUtils.GetLongestLength(values);
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
    
    public static void AddNoNew<T>(params T[][] values) where T : struct, IAdditionOperators<T, T, T>
    {
        long length = ArrayUtils.GetLongestLength(values);
        T[] totals = new T[length];

        for (long i = 0; i < values.LongLength; i++)
        {
            T[] subArray = values[i];
            for (long y = 0; y < subArray.LongLength; y++)
            {
                totals[y] += subArray[y];
            }
        }

        for (long i = 0; i < values.LongLength; i++)
        {
            T[] subArray = values[i];
            for (long y = 0; y < subArray.LongLength; y++)
            {
                subArray[y] = totals[y];
            }
        }
    }

    public static void MultiplyNoNew<T>(T[] values, T multiplier) where T : struct, IMultiplyOperators<T, T, T>
    {
        for (long i = 0; i < values.LongLength; i++)
            values[i] *= multiplier;
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

    /// <summary>
    /// area = x * y
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static T SquareArea<T>(T x, T y) where T : IMultiplyOperators<T, T, T> => x * y;

    /// <summary>
    /// area = (@base * height) / 2
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="base"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static T TriangleArea<T>(T @base, T height) where T : IMultiplyOperators<T, T, T>, IDivisionOperators<T, T, T>, INumber<T>
    {
        T squareArea = SquareArea(@base, height);
        T area = squareArea / T.CreateChecked(2);
        return area;
    }

    /// <summary>
    /// area = ((top + bottom) / 2) * height
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="top"></param>
    /// <param name="bottom"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static T TrapeziumArea<T>(T top, T bottom, T height) where T : IMultiplyOperators<T, T, T>, IDivisionOperators<T, T, T>, INumber<T>
    {
        T topBottomSum = (top + bottom) / T.CreateChecked(2);
        T result = topBottomSum * height;
        return result;
    }

    public static (T, string) GetApprioateUnit<T>(T bytes) where T : INumber<T>, IComparisonOperators<T, T, bool>
    {
        string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        T kilo = T.CreateChecked(1024);
        int unitIndex = 0;

        while (bytes >= kilo && unitIndex < units.Length - 1)
        {
            bytes /= kilo;
            unitIndex++;
        }

        return (bytes, units[unitIndex]);
    }
}