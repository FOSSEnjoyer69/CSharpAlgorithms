using CSharpAlgorithms.Interfaces;

namespace CSharpAlgorithms.Math;
public sealed class RangeFloat : Range<float>, IByteSize
{
    public uint GetByteSize() => sizeof(float) * 2;
}