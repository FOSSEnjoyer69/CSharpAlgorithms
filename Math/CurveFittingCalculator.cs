using System.Numerics;
using SixLaborsRgba32 = SixLabors.ImageSharp.PixelFormats.Rgba32;

namespace CSharpAlgorithms.Math;
public static class CurveFittingCalculator
{
    public static T LinerInterpolation<T>(T a, T b, T t) where T : INumber<T> => a + (b - a) * t;
    public static T LinerInterpolation<T>(T a, T b, float t) where T : INumber<T>, IMultiplyOperators<T, float, T> => a + (b - a) * t;
    public static byte LinerInterpolation(byte a, byte b, float t) =>  (byte)(a + (b - a) * t);
    public static SixLaborsRgba32 LinerInterpolation(SixLaborsRgba32 a, SixLaborsRgba32 b, float t)
    {
        return new SixLaborsRgba32()
        {
            R = LinerInterpolation(a.R, b.R, t),
            G = LinerInterpolation(a.G, b.G, t),
            B = LinerInterpolation(a.B, b.B, t),
            A = LinerInterpolation(a.A, b.A, t),
        };
    }
}