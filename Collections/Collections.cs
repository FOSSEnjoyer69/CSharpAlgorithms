using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CSharpAlgorithms.Collections;

public static class CollectionUtils
{
    public static void Clear<T>(BlockingCollection<T> blockingCollection)
    {
        while (blockingCollection.TryTake(out _)) { }
    }
    public static T[] CombineArray<T>(T[][] arrays)
    {
        int length = 0;
        for (int i = 0; i < arrays.Length; i++)
            length += arrays[i].Length;

        T[] combined = new T[length];
        int offset = 0;

        for (int i = 0; i < arrays.Length; i++)
        {
            Array.Copy(arrays[i], 0, combined, offset, arrays[i].Length);
            offset += arrays[i].Length;
        }

        return combined;
    }

    public static long GetLongestLength<T>(params T[][] array)
    {
        long longest = 0;

        for (long i = 0; i < array.LongLength; i++)
        {
            long length = array[i].LongLength;

            if (length > longest)
                longest = length;
        }

        return longest;
    }
    public static string ToString<T>(T[] array)
    {
        if (array is null)
            return "[]";

        string text = "[";

        for (int i = 0; i < array.Length; i++)
        {
            T item = array[i];
            string? itemString = item?.ToString();

            text += itemString;

            //Is not last element
            if (i != (array.Length - 1))
                text += ", ";
        }

        text += "]";

        return text;
    }
    public static T2 GetDictionaryValueAtindex<T1, T2>(Dictionary<T1, T2> dict, int index) where T1 : notnull
                                                                                           where T2 : notnull
    {
        if (index < 0 || index >= dict.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        int i = 0;
        foreach (var kvp in dict)
        {
            if (i == index)
                return kvp.Value;
            i++;
        }
        throw new InvalidOperationException("Index not found.");
    }
    public static bool TryGetIndex<T>(T[] values, T value, out int index)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i].Equals(value))
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    public static T[] Repeat<T>(T value, uint count)
    {
        if (count == 0)
            return [];

        T[] result = new T[count];
        for (uint i = 0; i < count; i++)
        {
            result[i] = value;
        }
        return result;
    }
}