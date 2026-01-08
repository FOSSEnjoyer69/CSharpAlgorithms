using System;
using CSharpAlgorithms.Math;
using SixLabors.ImageSharp.PixelFormats;

using SystemColour = System.Drawing.Color;

namespace CSharpAlgorithms.Media;

public static class ColourConverter
{
    public static HSLColour RGB_To_HSL(Colour colour) => RGB_To_HSL(colour.r, colour.g, colour.b);
    public static HSLColour RGB_To_HSL(float r, float g, float b)
    {

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));

        float hue = 0;
        float saturation = 0;
        float luminance = (max + min) * 0.5f;

        if (max == min)
        {
            hue = 0f;
            saturation = 0f;
            return new HSLColour(hue, saturation, luminance);
        }

        float d = max - min;
        saturation = luminance > 0.5f ? d / (2f - max - min) : d / (max + min);

        if (max == r)
            hue = (g - b) / d + (g < b ? 6f : 0f);
        else if (max == g)
            hue = (b - r) / d + 2f;
        else
            hue = (r - g) / d + 4f;

        hue /= 6f;

        return new HSLColour(hue, saturation, luminance);
    }

    public static Colour HSL_To_RGB(HSLColour colour)
    {
        if (colour.Saturation == 0f)
            return new Colour(colour.Luminance, colour.Luminance, colour.Luminance);

        float luminance = colour.Luminance;
        float q = luminance < 0.5f ? luminance * (1f + colour.Saturation) : luminance + colour.Saturation - luminance * colour.Saturation;
        float p = 2f * luminance - q;

        float r = HueToRgb(p, q, colour.Hue + 1f / 3f);
        float g = HueToRgb(p, q, colour.Hue);
        float b = HueToRgb(p, q, colour.Hue - 1f / 3f);

        return new Colour(r, g, b);
    }

    public static Rgba32 HSV_To_RGBA32(HSVColour colour, byte alpha)
    {
        colour.Hue %= 360f;
        if (colour.Hue < 0f) colour.Hue += 360f;

        colour.Saturation = Calculator.ClampInclusive(colour.Saturation, 0f, 1f);
        colour.Value = Calculator.ClampInclusive(colour.Value, 0f, 1f);

        float c = colour.Value * colour.Saturation;
        float hue = colour.Hue / 60f;
        float x = c * (1f - MathF.Abs((hue % 2f) - 1f));
        float m = colour.Value - c;

        float r1, g1, b1;
        if (hue < 1f) (r1, g1, b1) = (c, x, 0f);
        else if (hue < 2f) (r1, g1, b1) = (x, c, 0f);
        else if (hue < 3f) (r1, g1, b1) = (0f, c, x);
        else if (hue < 4f) (r1, g1, b1) = (0f, x, c);
        else if (hue < 5f) (r1, g1, b1) = (x, 0f, c);
        else (r1, g1, b1) = (c, 0f, x);

        byte R = (byte)MathF.Round((r1 + m) * 255f);
        byte G = (byte)MathF.Round((g1 + m) * 255f);
        byte B = (byte)MathF.Round((b1 + m) * 255f);

        return new Rgba32(R, G, B, alpha);
    }

    public static HSVColour ToHSV(SystemColour colour) => ToHSV(colour.R, colour.G, colour.B);
    public static HSVColour ToHSV(byte r, byte g, byte b)
    {
        byte hue = 0, saturation = 0, value = 0;

        float rf = r / 255f;
        float gf = g / 255f;
        float bf = b / 255f;

        float max = MathF.Max(rf, MathF.Max(gf, bf));
        float min = MathF.Min(rf, MathF.Min(gf, bf));
        float delta = max - min;

        // Calculate hue
        float hueTemp = 0f;
        if (delta != 0f)
        {
            if (max == rf)
                hueTemp = 60f * (((gf - bf) / delta) % 6f);
            else if (max == gf)
                hueTemp = 60f * (((bf - rf) / delta) + 2f);
            else
                hueTemp = 60f * (((rf - gf) / delta) + 4f);
        }
        if (hueTemp < 0f) hueTemp += 360f;
        hue = (byte)MathF.Round(hueTemp);

        // Calculate saturation
        saturation = (byte)MathF.Round(max == 0f ? 0f : (delta / max) * 255f);

        // Calculate value
        value = (byte)MathF.Round(max * 255f);

        return new HSVColour(hue, saturation, value);
    }

    public static float HueToRgb(float p, float q, float t)
    {
        if (t < 0f) t += 1f;
        if (t > 1f) t -= 1f;

        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }
}