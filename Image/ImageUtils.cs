using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Svg;

using ScottPlotImage = ScottPlot.Image;

namespace CSharpAlgorithms.Images;

public static class ImageUtils
{
    public static Bitmap SvgToImage(string svgPath, int width, int height)
    {
        SvgDocument svg = SvgDocument.Open(svgPath);

        svg.Width = width;
        svg.Height = height;

        Bitmap bitmap = svg.Draw();
        return bitmap;
    }

    public static byte[] GetBitmapBytes(Bitmap bitmap, ImageFormat format)
    {
        using (var ms = new MemoryStream())
        {
            bitmap.Save(ms, format); // Save as PNG, JPEG, BMP, etc.
            return ms.ToArray();
        }
    }

    public static ScottPlotImage ToScottPlotImage(Bitmap bitmap)
    {
        byte[] data = GetBitmapBytes(bitmap, ImageFormat.Png);
        ScottPlotImage image = new ScottPlotImage(data);
        return image;
    }
}