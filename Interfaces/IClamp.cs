namespace CSharpAlgorithms.Interfaces;
public interface IClamp<TMin, TMax>
{
    void Clamp(TMin min, TMax max);
}