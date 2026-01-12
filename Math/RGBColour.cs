using System.Numerics;

using SixLaborsRgba32 = SixLabors.ImageSharp.PixelFormats.Rgba32;

namespace CSharpAlgorithms.Math;
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