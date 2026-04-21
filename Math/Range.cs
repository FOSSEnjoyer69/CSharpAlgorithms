using System.Numerics;

namespace CSharpAlgorithms.Math;
public class Range<T> where T : struct, INumber<T>
{
    public T Min, Max;

    public Range(T min = default, T max = default)
    {
        Min = min;
        Max = max;
    }

    public Range(BinaryReader reader)
    {
        Min = T.CreateChecked(reader.ReadSingle());
        Max = T.CreateChecked(reader.ReadSingle());
    }

    public void BinarySerialize(BinaryWriter writer)
    {
        writer.Write(Convert.ToSingle(Min));
        writer.Write(Convert.ToSingle(Max));
    }

    public override string ToString()
    {
        return $"[CSharpAlgorithms.Math.Range<{typeof(T).Name}>: Min={Min}, Max={Max}]";
    }
}