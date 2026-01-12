using System;
using System.Collections.Generic;
using System.IO;
using TelltaleTextureTool.DirectX;
using TelltaleTextureTool.Main;
using TelltaleTextureTool.Utilities;
using System.Threading;
using System.Linq;
using System.ComponentModel;
using Hexa.NET.DirectXTex;
using TelltaleTextureTool.Telltale.FileTypes.D3DTX;
using TelltaleTextureTool.TelltaleEnums;
using TelltaleTextureTool.Graphics;
using TelltaleTextureTool;

namespace CSharpAlgorithms.Media.Images;

public static class ImageConverter
{

    public static void DDS_To_D3DTX(string ddsFilePath, string jsonFilePath="", string d3dtxFilepath="")
    {
        if (string.IsNullOrEmpty(ddsFilePath))
            return;

        if (string.IsNullOrEmpty(jsonFilePath))
        {
            FileInfo ddsFile = new FileInfo(ddsFilePath);
            string pathWithoutExtension = ddsFile.FullName.Replace(ddsFile.Extension, "");
            jsonFilePath = $"{pathWithoutExtension}.json";
        }

        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine($"could not find .json file at {jsonFilePath}");
            return;
        }

        // Create a new d3dtx object
        D3DTX_Master d3dtxMaster = new();

        // Parse the .json file as a d3dtx
        try
        {
            d3dtxMaster.ReadD3DTXJSON(jsonFilePath);
        }
        catch (Exception)
        {
            throw new Exception("Conversion failed.\nFailed to read the .d3dtx file.");
        }

        // If the d3dtx is a legacy D3DTX, force the use of the DX9 legacy flag
        DDSFlags flags = d3dtxMaster.IsLegacyD3DTX() ? DDSFlags.ForceDx9Legacy : DDSFlags.None;

        Texture texture = new(ddsFilePath, TextureType.DDS, flags);
        ImageAdvancedOptions options = new();


        // Set the options for the converter
        if (d3dtxMaster.d3dtxMetadata.TextureType is T3TextureType.eTxBumpmap or
                        T3TextureType.eTxNormalMap)
        {
            options.IsTelltaleNormalMap = true;
        }
        else if (d3dtxMaster.d3dtxMetadata.TextureType is T3TextureType.eTxNormalXYMap)
        {
            options.IsTelltaleNormalMap = true;
        }

        if (d3dtxMaster.d3dtxMetadata.SurfaceGamma is T3SurfaceGamma.sRGB)
        {
            options.IsSRGB = true;
        }

        texture.TransformTexture(options, true, true);

        // Get the image
        texture.GetDDSInformation(out D3DTXMetadata metadata, out ImageSection[] sections, flags);

        if (options.EnableSwizzle)
        {
            metadata.Platform = options.PlatformType;
        }

        // Modify the d3dtx file using our dds data
        d3dtxMaster.ModifyD3DTX(metadata, sections);

        texture.Release();

        // Write our final d3dtx file to disk
        d3dtxMaster.WriteFinalD3DTX(d3dtxFilepath);

    }

}