namespace CSharpAlgorithms.Audio;

public readonly struct AudioDeviceData
{
    public readonly string Name;
    public readonly bool HasInput, HasOutput;
    public readonly float Volume;
    public readonly bool IsMuted;

    public AudioDeviceData(AudioDevice device)
    {
        Name = device.Info.name;
        Volume = device.Volume;
        IsMuted = device.IsMuted;
    }

    public override string ToString()
    {
        return $"""
            Name: {Name}
            Volume: {Volume} 
            IsMuted: {IsMuted}
        """;
    }

    public static void Save(string filePath, AudioDeviceData[] deviceDatas)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(deviceDatas, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(filePath, json);
    }

    public static AudioDeviceData[] Load(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
            return [];

        string json = System.IO.File.ReadAllText(filePath);
        AudioDeviceData[]? deviceDatas = System.Text.Json.JsonSerializer.Deserialize<AudioDeviceData[]>(json);
        if (deviceDatas is null)
            return [];

        return deviceDatas;
    }
}