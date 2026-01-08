using System.Collections.Generic;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Biology.Animalia.Chordata.Mammalia.Primates.Hominidae.Homo.HomoSapiens;
public static class HomoSapiensEyes
{
    public static readonly Dictionary<string, Colour> ColourDictionary = new()
    {
        { "Blue",  new Colour(0f / 255f, 112f / 255f, 221f / 255f) },
        { "Green", new Colour(34f / 255f, 177f / 255f, 76f / 255f) },
        { "Brown", new Colour(136f / 255f, 78f / 255f, 34f / 255f) },
        { "Grey",  new Colour(128f / 255f, 128f / 255f, 128f / 255f) },
        { "Hazel", new Colour(153f / 255f, 101f / 255f, 21f / 255f) },
    };
}