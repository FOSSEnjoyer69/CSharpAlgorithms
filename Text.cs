using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpAlgorithms;

public static class Text
{
    public static string ReadNullTerminatedString(BinaryReader reader)
    {
        List<byte> bytes = new();
        byte b;
        while ((b = reader.ReadByte()) != 0)
            bytes.Add(b);
        return Encoding.ASCII.GetString(bytes.ToArray());
    }


    public static string ReadNullTerminatedString(byte[] data, int startIndex)
    {
        if (startIndex < 0 || startIndex >= data.Length)
            return string.Empty;

        int end = System.Array.IndexOf(data, (byte)0, startIndex);

        if (end == -1)
            end = data.Length;
            
        int length = end - startIndex;

        return Encoding.ASCII.GetString(data, startIndex, length);
    }
}