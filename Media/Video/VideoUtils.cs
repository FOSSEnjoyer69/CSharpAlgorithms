using System.Globalization;

namespace CSharpAlgorithms.Media.Video;

public static class VideoUtils
{
    public static bool TryParseFps(string text, out double fps)
    {
        fps = 0;

        if (string.IsNullOrWhiteSpace(text) || text == "0/0")
            return false;

        string[] parts = text.Split('/');

        if (parts.Length == 1)
        {
            return double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out fps);
        }

        if (parts.Length != 2)
            return false;

        if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double numerator))
            return false;

        if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double denominator))
            return false;

        if (denominator == 0)
            return false;

        fps = numerator / denominator;
        return true;
    }
}