namespace CSharpAlgorithms.Math;

using CSharpAlgorithms.Media;

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

    public static implicit operator HSLColour(Colour colour)
    {
        return ColourConverter.RGB_To_HSL(colour);
    }

}