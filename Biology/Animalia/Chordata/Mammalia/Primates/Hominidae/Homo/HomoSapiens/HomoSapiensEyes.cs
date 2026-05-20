using CSharpAlgorithms.Colour;

namespace CSharpAlgorithms.Biology.Animalia.Chordata.Mammalia.Primates.Hominidae.Homo.HomoSapiens;
public static class HomoSapiensEyes
{
    public static readonly Dictionary<string, RGBAColour<float>> ColourDictionary = new()
    {
        { "Blue",  new RGBAColour<float>(0f / 255f, 112f / 255f, 221f / 255f, 1f) },
        { "Green", new RGBAColour<float>(34f / 255f, 177f / 255f, 76f / 255f, 1f) },
        { "Brown", new RGBAColour<float>(136f / 255f, 78f / 255f, 34f / 255f, 1f) },
        { "Grey",  new RGBAColour<float>(128f / 255f, 128f / 255f, 128f / 255f, 1f) },
        { "Hazel", new RGBAColour<float>(153f / 255f, 101f / 255f, 21f / 255f, 1f) },
    };
}