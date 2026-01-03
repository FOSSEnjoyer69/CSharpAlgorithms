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
}