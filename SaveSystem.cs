#pragma warning disable

using System.Text.Json;

namespace CSharpAlgorithms;

public static class SaveSystem
{
    public static void SaveAsJson<T>(string fileName, T data)
    {
        string? directory = Path.GetDirectoryName(fileName);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
    }
    public static T LoadJson<T>(string fileName)
    {
        string json = File.ReadAllText(fileName);
        return JsonSerializer.Deserialize<T>(json);
    }
}