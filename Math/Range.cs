using System.Globalization;
using System.Numerics;

namespace CSharpAlgorithms.Math;
public class Range<T> where T : struct, INumber<T>, IParsable<T>
{
    public T Min, Max;
    public T Length => Max - Min;

    public Range(T min = default, T max = default)
    {
        if (min > max)
            throw new ArgumentException($"Min value cannot be greater than Max value. Received: Min={min}, Max={max}");

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









    public static bool Get<T>(T min, T max, out Range<T> range) where T : struct, INumber<T>, IParsable<T>
    {
        if (min >= max)
        {
            range = default;
            return false;
        }

        range = new Range<T>(min, max);
        return true;
    }

    public static void SaveToTextFile(string filePath, Range<T>[] ranges)
    {
        using StreamWriter writer = new(filePath);
        foreach (Range<T> range in ranges)
        {
            writer.WriteLine($"{range.Min},{range.Max}");
        }
    }

    public static bool LoadFromTextFile(string filePath, out Range<T>[] ranges)
    {
        const string CALL_PATH = "CSharpAlgorithms.Math.Range.LoadFromTextFile";
        
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            ranges = new Range<T>[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length != 2)
                    throw new FormatException($"Invalid line format: {lines[i]}");

                T min = T.Parse(parts[0], CultureInfo.InvariantCulture);
                T max = T.Parse(parts[1], CultureInfo.InvariantCulture);
                ranges[i] = new Range<T>(min, max);
            }
            return true;
        }
        catch
        {
            ranges = [];
            return false;
        }
    }
}