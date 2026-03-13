#define DEBUG

using System;
using System.Linq;
using CSharpAlgorithms.Collection;


namespace CSharpAlgorithms.Audio;

public readonly struct AudioDeviceData
{
    public readonly string Name {get; init;}
    public readonly float InputVolume {get; init;}
    public readonly float OutputVolume {get; init;}
    public readonly bool IsMuted {get; init;}

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
        if (deviceDatas is null)
        {
            if (!Get(null!, out deviceDatas))
                return;
        }

        string json = System.Text.Json.JsonSerializer.Serialize(deviceDatas, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(filePath, json);

#if DEBUG
        Debug.WriteLine($"Saved to {filePath}");
        Debug.WriteLine(json);
#endif
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

    public static bool Get(AudioDevice[] devices, out AudioDeviceData[] deviceDatas)
    {
        devices ??= DictionaryUtils.GetValues(AudioDevice.ActiveDevices);
        deviceDatas = [.. devices.Select(device => new AudioDeviceData(device))];
        return true;
    }
}