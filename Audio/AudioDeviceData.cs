using System;
using System.Linq;
using CSharpAlgorithms.Collection;

namespace CSharpAlgorithms.Audio;

public readonly struct AudioDeviceData
{
    public readonly string Name;
    public readonly bool HasInput, HasOutput;
    public readonly float InputVolume, OutputVolume;
    public readonly bool IsMuted;

    public const string DEFAULT_FILE_PATH = "devices.json";

    public AudioDeviceData(AudioDevice device)
    {
        Name = device.Info.name;
        InputVolume = device.InputVolume;
        OutputVolume = device.OutputVolume;
        IsMuted = device.IsMuted;
    }

    public override string ToString()
    {
        return $"""
            Name: {Name}
            Volume: {InputVolume} 
            Volume: {OutputVolume} 
            IsMuted: {IsMuted}
        """;
    }

    public static void Save(string filePath = DEFAULT_FILE_PATH, AudioDeviceData[] deviceDatas= null!)
    {
        deviceDatas ??= Get();

        string json = System.Text.Json.JsonSerializer.Serialize(deviceDatas, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(filePath, json);
        Console.WriteLine($"Saved audio devices to {filePath}");
    }

    public static AudioDeviceData[] Load(string filePath = DEFAULT_FILE_PATH)
    {
        if (!System.IO.File.Exists(filePath))
            return [];

        string json = System.IO.File.ReadAllText(filePath);
        AudioDeviceData[]? deviceDatas = System.Text.Json.JsonSerializer.Deserialize<AudioDeviceData[]>(json);
        if (deviceDatas is null)
            return [];

        return deviceDatas;
    }

    public static AudioDeviceData[] Get(AudioDevice[] devices = null!)
    {
        devices ??= DictionaryUtils.GetValues(AudioDevice.ActiveDevices);
        return [.. devices.Select(device => new AudioDeviceData(device))];
    }
}