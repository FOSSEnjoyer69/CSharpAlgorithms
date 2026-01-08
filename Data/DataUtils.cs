using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAlgorithms.Data;

public static class DataUtils
{
    public static uint GetMaskOfBitsToKeep(uint bitOffsetStart, uint bitsToKeep)
    {
        uint mask = (uint)((1UL << (int)bitsToKeep) - 1);
        return mask << (int)bitOffsetStart;
    }

    public static uint KeepBitsOfValue(uint sourceData, uint bitsToKeepOffsetStart, uint amountOfBitsToKeep) => sourceData & GetMaskOfBitsToKeep(bitsToKeepOffsetStart, amountOfBitsToKeep);
    public static uint ExtractBits(uint sourceData, uint bitOffsetStart, uint bitsToExtract)
    {
        uint bitmask = (1u << (int)bitsToExtract) - 1u;
        return (sourceData >> (int)bitOffsetStart) & bitmask;
    }
    public static bool IsBitAtOffsetSet(uint sourceData, uint bitOffsetLocation) => ExtractBits(sourceData, bitOffsetLocation, 1u) != 0;
    public static uint ClearBitAtOffset(uint sourceData, uint bitOffsetLocation) => sourceData &= ~(1u << (int)bitOffsetLocation);
    public static uint CombineBits(uint sourceData, uint sourceDataBitSize, uint newData, uint newDataBitSize)
    {
        uint bitsA = KeepBitsOfValue(sourceData, 0, sourceDataBitSize);
        uint bitsB = KeepBitsOfValue(newData, 0, newDataBitSize);
        return bitsA | (bitsB << (int)sourceDataBitSize);
    }

    public static uint GetByteArrayListElementsCount(List<byte[]> array) => (uint)array.Sum(x => x.Length);

    public static byte[] Combine(byte[] first, byte[] second)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        // Handle edge cases with empty arrays
        if (first.Length == 0)
            return (byte[])second.Clone();
        if (second.Length == 0)
            return (byte[])first.Clone();
        // Check for potential overflow
        checked
        {
            try
            {
                byte[] result = new byte[first.Length + second.Length];
                Buffer.BlockCopy(first, 0, result, 0, first.Length);
                Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
                return result;
            }
            catch (OverflowException)
            {
                throw new InvalidOperationException(
                    $"Combined array size exceeds maximum allowed length ({int.MaxValue} bytes)"
                );
            }
        }
    }
}