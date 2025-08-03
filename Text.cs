using System.Text.RegularExpressions;

namespace CSharpAlgorithms;

public static class Text
{
    public static string RemoveOrdinalSuffix(string input)
    {
        return Regex.Replace(input, @"\b(\d{1,2})(st|nd|rd|th)\b", "$1", RegexOptions.IgnoreCase);
    }

    public static string RemoveDay(string input)
    {
        return input.Replace("Monday", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("Tuesday", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("Wednesday", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("Thursday", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("Friday", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("Saturday", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("Sunday", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}