using System.Numerics;

using SixLaborsRgba32 = SixLabors.ImageSharp.PixelFormats.Rgba32;

namespace CSharpAlgorithms.Colour;
public struct RGBAColour<T> where T : INumber<T>
{
    public T R, G, B, A;

    public RGBAColour(T r, T g, T b, T a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public RGBAColour(byte[] bytes)
    {
        if (bytes.Length < 3)
            throw new ArgumentException("Byte array must have at least 3 elements for RGB.", nameof(bytes));

        R = T.CreateChecked(bytes[0]);
        G = T.CreateChecked(bytes[1]);
        B = T.CreateChecked(bytes[2]);

        if (bytes.Length > 3)
            A = T.CreateChecked(bytes[3]);
        else
            A = T.CreateChecked(255); // Default alpha to 255 if not provided
    }

    public static implicit operator RGBAColour<T>(SixLaborsRgba32 colour)
    {
        return new RGBAColour<T>
        {
            R = T.CreateChecked(colour.R),
            G = T.CreateChecked(colour.G),
            B = T.CreateChecked(colour.B),
            A = T.CreateChecked(colour.A)
        };
    }
}