using System;
using CSharpAlgorithms.Colour;
using CSharpAlgorithms.Math;

using SixLaborsImage = SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;


namespace CSharpAlgorithms.Media.Images;

using static CurveFittingCalculator;

public sealed class ImageEditor(SixLaborsImage image)
{
    public Image Image { get; private set; } = image;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ratio">0 is no colour (grey scale), 1 is the original, this is interpolated</param>
    /// <param name="mask">a mask to restrict the affected area, no mask will affect the whole image</param>
    //public void SetSaturation(float ratio, Image<Rgba32>? mask = null)
    //{
    //    Image.ProcessPixelRows(accessor =>
    //    {
    //        for (int y = 0; y < accessor.Height; y++)
    //        {
    //            var pixelRow = accessor.GetRowSpan(y);
    //            for (int x = 0; x < pixelRow.Length; x++)
    //            {
    //                Rgba32 originalPixel = pixelRow[x];
    //
    //                float maskFactor = 1f;
    //                if (mask != null)
    //                {
    //                    Rgba32 maskPixel = mask[x, y];
    //                    maskFactor = 1f - ((maskPixel.R + maskPixel.G + maskPixel.B) / (255f * 3f));
    //                }
    //
    //                float gray = (originalPixel.R + originalPixel.G + originalPixel.B) / 3;
    //
    //                byte r = (byte)LinerInterpolation(gray, originalPixel.R, maskFactor);
    //                byte g = (byte)LinerInterpolation(gray, originalPixel.G, maskFactor);
    //                byte b = (byte)LinerInterpolation(gray, originalPixel.B, maskFactor);
    //
    //                pixelRow[x] = new Rgba32(r, g, b, originalPixel.A);
    //            }
    //        }
    //    });
    //}

    public void Colourize(HSLColour colour, float opacity = 1f, Image? mask = null, float lightnessShift = 0f)
    {
        opacity = Calculator.ClampInclusive(opacity, 0f, 1f);
        if (opacity <= 0f) return;

        // Use target hue/sat from the chosen colour
        float targetHue = Wrap01(colour.Hue);
        float targetSat = Clamp01(colour.Saturation);

        // Optional: GIMP-like lightness shift slider in [-1..+1]
        lightnessShift = Calculator.ClampInclusive(lightnessShift, -1f, 1f);

        for (uint y = 0; y < Image.Height; y++)
        {
            for (uint x = 0; x < Image.Width; x++)
            {
                if (!Image.TryGetPixel(x, y, out RGBAColour<byte> pixel))
                    return;


                float maskFactor = 1f;
                if (mask is not null && mask.TryGetPixel(x, y, out RGBAColour<byte> maskPixel))
                    maskFactor = SrgbLuma01(maskPixel); // white=apply, black=skip

                float t = opacity * maskFactor;
                if (t <= 0f) continue;


                // Work in 0..1
                float sr = pixel.R / 255f;
                float sg = pixel.G / 255f;
                float sb = pixel.B / 255f;

                // Keep source shading
                HSLColour srcHsl = ColourConverter.RGB_To_HSL(sr, sg, sb);
                float outL = ApplyLightnessShift(srcHsl.Luminance, lightnessShift);

                // Replace hue/sat
                var outHsl = new HSLColour(targetHue, targetSat, outL);

                // Convert back to RGB (0..1 floats in your Colour struct)
                RGBAColour<float> outRgb = outHsl;

                // Blend in 0..1, then convert to bytes
                float fr = Lerp(sr, outRgb.R, t);
                float fg = Lerp(sg, outRgb.G, t);
                float fb = Lerp(sb, outRgb.B, t);

                Image.SetPixel(x, y, new RGBAColour<byte>(ToByte(fr), ToByte(fg), ToByte(fb), pixel.A));



            }
        }
    }

    private static byte ToByte(float v)
    {
        v = Clamp01(v);
        return (byte)Calculator.ClampInclusive((int)MathF.Round(v * 255f), 0, 255);
    }

    private static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);
    private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
    private static float Wrap01(float v) { v %= 1f; if (v < 0f) v += 1f; return v; }
    private static float SrgbLuma01(RGBAColour<byte> c)
    {
        // simple luma for masks (good enough for grayscale masks)
        float r = c.R / 255f;
        float g = c.G / 255f;
        float b = c.B / 255f;
        return Calculator.ClampInclusive(0.299f * r + 0.587f * g + 0.114f * b, 0, 1);
    }

    private static float ApplyLightnessShift(float l, float shift)
    {
        // shift in [-1..+1]
        // +1 => white, -1 => black, 0 => unchanged
        if (shift >= 0f)
            return l + (1f - l) * shift;   // move toward 1
        else
            return l * (1f + shift);       // move toward 0 (shift is negative)

        // result stays within [0..1] automatically
    }
}