using System;
using System.IO;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using CSharpAlgorithms.Math;
using BCnEncoder.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using SixLaborsImage = SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;
using TelltaleTextureTool.Main;
using Hexa.NET.DirectXTex;
using TelltaleTextureTool.DirectX;
using TelltaleTextureTool.TelltaleEnums;
using TelltaleTextureTool;
using TelltaleTextureTool.Telltale.FileTypes.D3DTX;

namespace CSharpAlgorithms.Media.Images;

public sealed class Image
{
    public RGBAColour<byte>[] Pixels { get; private set; }
    public uint Width { get; private set; }
    public uint Height { get; private set; }

    public Image(int width, int height)
    {
        int pixelCount = width * height;
        Pixels = new RGBAColour<byte>[pixelCount];
    }

    public Image(RGBAColour<byte>[] pixels, uint width, uint height)
    {
        Pixels = pixels;
        Width = width;
        Height = height;
    }

    public bool TryGetPixel(uint x, uint y, out RGBAColour<byte> pixel)
    {
        bool inFrame = (x < Width) && (y < Height);
        if (!inFrame)
        {
            pixel = new RGBAColour<byte>();
            return false;
        }

        uint pixelIndex = (Width * x) + y;
        pixel = Pixels[pixelIndex];
        return true;
    }

    public void SetPixel(uint x, uint y, RGBAColour<byte> colour)
    {
        bool inFrame = (x < Width) && (y < Height);
        if (!inFrame)
            return;

        uint pixelIndex = (Width * x) + y;
        Pixels[pixelIndex] = colour;
    }

    public void SaveDDS(string filePath)
    {
        using SixLaborsImage image = this;

        BcEncoder encoder = new();
        encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;
        encoder.OutputOptions.Format = CompressionFormat.Bc3;
        encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
        encoder.OutputOptions.GenerateMipMaps = true;

        using FileStream stream = File.Create(filePath);
        encoder.EncodeToStream(image, stream);
    }

    public static Image LoadDDS(string filePath)
    {
        using Pfim.IImage pfim = Pfim.Pfimage.FromFile(filePath);
        if (pfim.Compressed)
            pfim.Decompress();

        return pfim.Format switch
        {
            Pfim.ImageFormat.Rgba32 => SixLaborsImage.LoadPixelData<Bgra32>(pfim.Data, pfim.Width, pfim.Height).CloneAs<Rgba32>(),
            Pfim.ImageFormat.Rgb24 => SixLaborsImage.LoadPixelData<Bgr24>(pfim.Data, pfim.Width, pfim.Height).CloneAs<Rgba32>(),

            _ => throw new NotSupportedException($"Unsupported DDS decoded format: {pfim.Format}")
        };
    }

    //The general idea is right but some modifcations need to be made for this to be ready
    //public void SaveD3DTX(string filePath, string jsonFilePath, ImageAdvancedOptions options)
    //{
    //    if (string.IsNullOrEmpty(filePath))
    //    {
    //        Console.WriteLine($"Cannot save image to {filePath}");
    //    }
//
    //    if (string.IsNullOrEmpty(jsonFilePath))
    //    {
    //        Console.WriteLine($"Cannot read .json from {jsonFilePath}");
    //    }
//
    //    D3DTX_Master d3dtx = new();
    //    d3dtx.ReadD3DTXJSON(jsonFilePath);
//
    //    DDSFlags flags = d3dtx.IsLegacyD3DTX() ? DDSFlags.ForceDx9Legacy : DDSFlags.None;
    //    Texture texture = new(filePath, TelltaleTextureTool.Graphics.TextureType.DDS, flags);
//
    //    // Set the options for the converter
    //    if (d3dtx.d3dtxMetadata.TextureType is T3TextureType.eTxBumpmap or
    //                    T3TextureType.eTxNormalMap)
    //    {
    //        options.IsTelltaleNormalMap = true;
    //    }
    //    else if (d3dtx.d3dtxMetadata.TextureType is T3TextureType.eTxNormalXYMap)
    //    {
    //        options.IsTelltaleNormalMap = true;
    //    }
//
    //    if (d3dtx.d3dtxMetadata.SurfaceGamma is T3SurfaceGamma.sRGB)
    //    {
    //        options.IsSRGB = true;
    //    }
//
    //    texture.TransformTexture(options, true, true);
    //    texture.GetDDSInformation(out D3DTXMetadata metadata, out ImageSection[] sections, flags);
//
    //    if (options.EnableSwizzle)
    //        metadata.Platform = options.PlatformType;
//
    //    d3dtx.ModifyD3DTX(metadata, sections);
    //    texture.Release();
//
    //    d3dtx.WriteFinalD3DTX(filePath);
    //}

    #region  Operators
    public static implicit operator Image(SixLaborsImage inputImage)
    {
        int pixelCount = inputImage.Width * inputImage.Height;
        var Pixels = new RGBAColour<byte>[pixelCount];

        inputImage.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgba32> row = accessor.GetRowSpan(y);

                for (int x = 0; x < row.Length; x++)
                {
                    Rgba32 src = row[x];

                    int pixelIndex = (inputImage.Width * x) + y;
                    Pixels[pixelIndex] = src;
                }
            }
        });

        return new Image(Pixels, (uint)inputImage.Width, (uint)inputImage.Height);
    }
    public static implicit operator SixLaborsImage(Image image)
    {
        int width = (int)image.Width;
        int height = (int)image.Height;

        SixLaborsImage output = new(width, height);

        output.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgba32> row = accessor.GetRowSpan(y);

                for (int x = 0; x < row.Length; x++)
                {
                    if (image.TryGetPixel((uint)x, (uint)y, out RGBAColour<byte> pixel))
                    {
                        row[x] = new Rgba32()
                        {
                            R = pixel.R,
                            G = pixel.G,
                            B = pixel.B,
                            A = pixel.A,
                        };
                    }
                }
            }
        });

        return output;
    }
    #endregion
}