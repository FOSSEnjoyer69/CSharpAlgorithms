using System;
using System.IO;
using System.Text;

namespace CSharpAlgorithms.Data;
public static class BinaryUtils
{
    public static void WriteTelltaleBoolean(this BinaryWriter writer, bool Boolean) => WriteBooleanAsChar(writer, Boolean);
    public static void WriteBooleanAsChar(this BinaryWriter writer, bool Boolean)
    {
        char character = Boolean ? '1' : '0';
        writer.Write(character);
    }

    public static bool ReadTelltaleBoolean(this BinaryReader reader) => ReadCharAsBoolean(reader);
    public static bool ReadCharAsBoolean(this BinaryReader reader)
    {
        char character = reader.ReadChar();

        return character switch
        {
            '1' => true,
            '0' => false,
            _ => throw new Exception($"Invalid Telltale Boolean data. value was {character} instead of '0' or '1'"),
        };
    }

    /// <summary>
    /// writes the length of the string then the string
    /// </summary>
    /// <param name="writer">the writer</param>
    /// <param name="value">the string</param>
    public static void WriteInt32LengthPrefixedString(this BinaryWriter writer, string value)
    {
        writer.Write(value.Length);
        writer.Write(value);
    }
    public static string ReadInt32LengthPrefixedString(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        return ReadKnownLengthString(reader, length);
    }


    public static string ReadKnownLengthString(this BinaryReader reader, int length)
    {
        StringBuilder builder = new(capacity: length);

        for (int i = 0; i < length; i++)
            builder.Append(reader.ReadChar());

        return builder.ToString();
    }

    public static bool TryWriteGeneric<T>(this BinaryWriter writer, T value)
    {
        switch (value)
        {
            case byte v:    writer.Write(v); return true;
            case sbyte v:   writer.Write(v); return true;
            case short v:   writer.Write(v); return true;
            case ushort v:  writer.Write(v); return true;
            case int v:     writer.Write(v); return true;
            case uint v:    writer.Write(v); return true;
            case long v:    writer.Write(v); return true;
            case ulong v:   writer.Write(v); return true;
            case float v:   writer.Write(v); return true;
            case double v:  writer.Write(v); return true;
            case bool v:    writer.Write(v); return true;
            case char v:    writer.Write(v); return true;
            case string v:  writer.Write(v); return true;
            default:
                return false;
        }
    }
}