using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Colour;
public static class ColourConverter
{
    public static byte[][] ToByteArrays(RGBAColour<byte>[] colours)
    {
        byte[][] byteList = new byte[colours.Length][];
        for (int i = 0; i < colours.Length; i++)
        {
            byte r = colours[i].R;
            byte g = colours[i].G;
            byte b = colours[i].B;
            byte a = colours[i].A;

            byteList[i] = [r, g, b, a];
        }

        return byteList;
    }
    public static HSLColour RGB_To_HSL(RGBAColour<float> colour) => RGB_To_HSL(colour.R, colour.G, colour.B);
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
}