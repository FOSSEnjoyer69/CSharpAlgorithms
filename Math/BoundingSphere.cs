using System.IO;
using System.Numerics;
using CSharpAlgorithms.Interfaces;

namespace CSharpAlgorithms.Math;

public class Sphere<T> : IByteSize
    where T : struct, INumber<T>
{
    public Vector3<T> Center;
    public T Radius;

    private uint blockSize = 20;

    public Sphere(Vector3<T> center, T radius)
    {
        Center = center;
        Radius = radius;
    }

    public Sphere(BinaryReader reader)
    {
        reader.ReadUInt32();

        Center = new Vector3<T>(reader);
        Radius = T.CreateChecked(reader.ReadSingle());
    }

    public void UpdateStructure()
    {
        blockSize = 4 + 12 + 4;
    }

    public void BinarySerialize(BinaryWriter writer)
    {
        writer.Write(blockSize);
        Center.BinarySerialize(writer);
        writer.Write((dynamic)Radius);
    }

    public void SetBoundingSphereBasedOnBoundingBox(BoundingBox<T> boundingBox)
    {
        Center = boundingBox.Center;
        Radius = boundingBox.Size.Magnitude;;
    }

    public override string ToString()
    {
        return $"[CSharpAlgorithms.Math.Sphere<{typeof(T).Name}>: Center={Center}, Radius={Radius}]";
    }

    #pragma warning disable
    public unsafe uint GetByteSize() => (uint)(sizeof(T) + sizeof(uint) + Center.GetByteSize());
    #pragma warning restore
    
}