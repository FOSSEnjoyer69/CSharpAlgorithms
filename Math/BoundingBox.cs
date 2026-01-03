using System.IO;
using System.Numerics;
using CSharpAlgorithms.Interfaces;

namespace CSharpAlgorithms.Math;
public class BoundingBox<T> : IByteSize 
    where T : struct, INumber<T>
{
    public Vector3<T> Min, Max;
    public Vector3<T> Center => (Max + Min) * 0.5f;
    public Vector3<T> Size => Max - Min;
    public Vector3<T> Extents => Size * 0.5f;

    public BoundingBox(Vector3<T> min, Vector3<T> max)
    {
        Min = min;
        Max = max;
    }

    public BoundingBox(BinaryReader reader)
    {
        Min = new Vector3<T>(reader);
        Max = new Vector3<T>(reader);
    }

    public void BinarySerialize(BinaryWriter writer)
    {
        Min.BinarySerialize(writer);
        Max.BinarySerialize(writer);
    }

    public override string ToString()
    {
        return $"[CSharpAlgorithms.Math.BoundingBox<{typeof(T).Name}>: Min={Min}, Max={Max}]";
    }

    public uint GetByteSize()
    {
        return Min.GetByteSize() + Max.GetByteSize();
    }
}