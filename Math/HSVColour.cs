namespace CSharpAlgorithms.Math;
public struct HSVColour()
{
    public float Hue, Saturation, Value;

    public HSVColour(float hue, float saturation, float value) : this()
    {
        Hue = Calculator.ClampInclusive(hue, 0, 1);
        Saturation = Calculator.ClampInclusive(saturation, 0, 1);
        Value = Calculator.ClampInclusive(value, 0, 1);
    }

    public override string ToString() => $"[CSharpAlgorithms.HSVColour] Hue: {Hue}, Saturation: {Saturation}, Value: {Value}";    
}