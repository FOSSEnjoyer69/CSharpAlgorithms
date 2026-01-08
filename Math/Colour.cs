using System.IO;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace CSharpAlgorithms.Math;

public struct Colour
{
    public float r;
    public float g;
    public float b;
    public float a;

    public Colour(float r=1, float g=1, float b=1, float a=1)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Colour(BinaryReader reader)
    {
        r = reader.ReadSingle();
        g = reader.ReadSingle();
        b = reader.ReadSingle();
        a = reader.ReadSingle();
    }

    public readonly void WriteBinaryData(BinaryWriter writer)
    {
        writer.Write(r);
        writer.Write(g);
        writer.Write(b);
        writer.Write(a);
    }

    public readonly uint GetByteSize()
    {
        uint totalByteSize = 0;

        totalByteSize += (uint)Marshal.SizeOf(r); //r [4 bytes]
        totalByteSize += (uint)Marshal.SizeOf(g); //g [4 bytes]
        totalByteSize += (uint)Marshal.SizeOf(b); //b [4 bytes]
        totalByteSize += (uint)Marshal.SizeOf(a); //a [4 bytes]

        return totalByteSize;
    }

    public override string ToString() => string.Format("[Color] r: {0} g: {1} b: {2} a: {3}", r, g, b, a);

    public static implicit operator Rgba32(Colour colour) => new(colour.r, colour.g, colour.b, colour.a);
    public static implicit operator Colour(Rgba32 rgba) => new(rgba.R / 255f, rgba.G / 255f, rgba.B / 255f, rgba.A / 255f);
    

}