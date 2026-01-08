using System.IO;
using System.Linq;
using TelltaleTextureTool.Codecs;
using TelltaleTextureTool.Graphics;
using TelltaleTextureTool.TelltaleEnums;

namespace CSharpAlgorithms.Media.Images;
public static class ImageConverter
{
    public static readonly string[] SupportedFormats = ["png", "dds", "d3dtx"];

    public static bool ConvertImageToFormat(string inputFilePath, string format)
    {
        if (SupportedFormats.Contains(format.ToLower()) == false)
            return false;
        
        string inputFormat = Path.GetExtension(inputFilePath)
                                 .ToLower()
                                 .Replace(".", "");

        return (inputFormat, format.ToLower()) switch
        {
            ("d3dtx", "dds") => D3DTX_To_DDS(inputFilePath),
            ("dds", "d3dtx") => ToD3DTX(inputFilePath),
            ("png", "dds") => false,
            ("png", "d3dtx") => false,
            _ => false,
        };
    }

    public static bool D3DTX_To_DDS(string inputFilePath)
    {
        FileInfo inputFile = new FileInfo(inputFilePath);
        if (!inputFile.Exists)
            return false;

        string inputFolderDirectory = inputFile.Directory?.FullName ?? "";
        string finalTexturePath = $"{inputFolderDirectory}/{Path.GetFileNameWithoutExtension(inputFile.Name)}.dds";

        CodecManager codecManager = new();
        CodecOptions codecOptions = new()
        {
            TelltaleToolGame = TelltaleToolGame.THE_WALKING_DEAD_DEFINITIVE_SERIES
        };

        Texture originalTexture = codecManager.LoadFromFile(inputFilePath, codecOptions);
        codecManager.SaveToFile(finalTexturePath, originalTexture, codecOptions);

        return true;
    }   

    public static bool ToD3DTX(string inputFilePath)
    {
        FileInfo inputFile = new FileInfo(inputFilePath);
        if (!inputFile.Exists)
            return false;

        string finalTexturePath = Path.GetFileNameWithoutExtension(inputFile.Name) + ".d3dtx";

        CodecManager codecManager = new();
        CodecOptions codecOptions = new()
        {
            TelltaleToolGame = TelltaleToolGame.THE_WALKING_DEAD_DEFINITIVE_SERIES
        };

        Texture originalTexture = codecManager.LoadFromFile(inputFilePath, codecOptions);
        codecManager.SaveToFile(finalTexturePath, originalTexture, codecOptions);

        return true;
    }
}