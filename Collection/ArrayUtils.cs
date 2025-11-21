
using System;
using System.Numerics;

namespace CSharpAlgorithms.Collection;
public static class ArrayUtils
{
    public static long GetLongestLength<T>(params T[][] arrays)
    {
        long length = 0;

        for (int i = 0; i < arrays.LongLength; i++)
        {
            if (arrays[i].LongLength > length)
                length = arrays[i].LongLength;
        }

        return length;
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

    public static void Stretch(float[] sourceCopy, int ratio)
    {
        ArgumentNullException.ThrowIfNull(sourceCopy);

        if (ratio <= 1)
            return;

        int originalLength = sourceCopy.Length;
        int newLength = originalLength * ratio;
        float[] stretched = new float[newLength];

        for (int i = 0; i < originalLength; i++)
        {
            for (int j = 0; j < ratio; j++)
            {
                stretched[i * ratio + j] = sourceCopy[i];
            }
        }

        // Copy back to sourceCopy (resize if needed)
        Array.Resize(ref sourceCopy, newLength);
        Array.Copy(stretched, sourceCopy, newLength);
    }

    public static void XOR<T>(T[] data, T xorNum) where T : IBitwiseOperators<T, T, T>
    {
        for (int i = 0; i < data.Length; i++)
            data[i] ^= xorNum;
    }
    
    public static void XOR<T1, T2>(T1[] data, T2 xorNum) where T1: IBitwiseOperators<T1, T2, T1>
    {
        for (int i = 0; i < data.Length; i++)
            data[i] ^= xorNum;
    }
}