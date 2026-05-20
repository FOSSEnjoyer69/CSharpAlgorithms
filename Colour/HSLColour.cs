using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Colour;

public struct HSLColour()
{
    public float Hue, Saturation, Luminance;

    public HSLColour(float hue, float saturation, float luminance) : this()
    {
        Hue = Calculator.ClampInclusive(hue, 0, 1);
        Saturation = Calculator.ClampInclusive(saturation, 0, 1);
        Luminance = Calculator.ClampInclusive(luminance, 0, 1);
    }

    public override string ToString() => $"[CSharpAlgorithms.HSLColour] Hue: {Hue}, Saturation: {Saturation}, Luminance: {Luminance}";

    public static implicit operator HSLColour(RGBAColour<float> colour)
    {
        float r = colour.R / 255f;
        float g = colour.G / 255f;
        float b = colour.B / 255f;

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        float delta = max - min;

        float hue = 0f;
        if (delta > 0)
        {
            if (max == r)
                hue = (g - b) / delta + (g < b ? 6 : 0);
            else if (max == g)
                hue = (b - r) / delta + 2;
            else
                hue = (r - g) / delta + 4;

            hue /= 6;
        }

        float luminance = (max + min) / 2f;
        float saturation = delta == 0 ? 0 : delta / (1 - MathF.Abs(2 * luminance - 1));

        return new HSLColour(hue, saturation, luminance);
    }

    public static implicit operator RGBAColour<float>(HSLColour hsl)
    {
        float r, g, b;

        if (hsl.Saturation == 0)
        {
            r = g = b = hsl.Luminance; // Achromatic (grey)
        }
        else
        {
            float q = hsl.Luminance < 0.5f
                ? hsl.Luminance * (1 + hsl.Saturation)
                : hsl.Luminance + hsl.Saturation - hsl.Luminance * hsl.Saturation;
            float p = 2 * hsl.Luminance - q;

            r = HueToRGB(p, q, hsl.Hue + 1f / 3f);
            g = HueToRGB(p, q, hsl.Hue);
            b = HueToRGB(p, q, hsl.Hue - 1f / 3f);
        }

        return new RGBAColour<float>(r * 255f, g * 255f, b * 255f, 255f);
    }

    private static float HueToRGB(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1f / 6f) return p + 6 * (q - p) * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + 6 * (q - p) * (2f / 3f - t);
        return p;
    }

}