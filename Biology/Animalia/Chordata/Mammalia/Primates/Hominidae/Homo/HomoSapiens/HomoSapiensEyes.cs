using System.Collections.Generic;
using System.Drawing;

namespace CSharpAlgorithms.Biology.Animalia.Chordata.Mammalia.Primates.Hominidae.Homo.HomoSapiens;
public static class HomoSapiensEyes
{
    public static readonly Dictionary<string, Color> ColourDictionary = new()
    {
        { "Blue", Color.FromArgb(255, 0, 112, 221) },
        { "Green", Color.FromArgb(255, 34, 177, 76) },
        { "Brown", Color.FromArgb(255, 136, 78, 34) },
        { "Grey", Color.FromArgb(255, 128, 128, 128) },
        { "Hazel", Color.FromArgb(255, 153, 101, 21) },
    };
}