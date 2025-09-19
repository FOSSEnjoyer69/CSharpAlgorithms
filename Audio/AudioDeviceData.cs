using CSharpAlgorithms.Collections;

namespace CSharpAlgorithms.Audio;

public struct AudioDeviceData : IMute, IVolume
{
    public string Name { get; set; } = string.Empty;
    public bool IsInput { get; set; }
    public float Volume { get; set; } = 1;
    public bool IsMuted { get; set; } = false;

    public string[] Tags { get; set; } = [];

    public static readonly AudioDeviceData DefaultInput = new() { Name = "Default", IsInput = true };
    public static readonly AudioDeviceData DefaultOutput = new() { Name = "Default", IsInput = false };

    public AudioDeviceData() { }
    public AudioDeviceData(AudioInputDevice device)
    {
        Name = device.Info.name;
        IsInput = true;
        Volume = device.Volume;
        IsMuted = device.IsMuted;
    }
    public AudioDeviceData(AudioOutputDevice device)
    {
        Name = device.Info.name;
        IsInput = false;
        Volume = device.Volume;
        IsMuted = device.IsMuted;
    }

    public override string ToString()
    {
        return $"""
            Name: {Name}
            IsInput: {IsInput} 
            Volume: {Volume} 
            IsMuted: {IsMuted}
            Tags: {CollectionUtils.ToString(Tags)}
        """;
    }

    public static AudioDeviceData[] FromMixer(AudioMixer mixer)
    {
        AudioDeviceData[] deviceDatas = new AudioDeviceData[mixer.inputDevices.Count + mixer.outputDevices.Count];

        for (int i = 0; i < mixer.inputDevices.Count; i++)
        {
            AudioInputDevice device = CollectionUtils.GetDictionaryValueAtindex(mixer.inputDevices, i);
            AudioDeviceData deviceData = new AudioDeviceData(device);
            deviceDatas[i] = deviceData;
        }

        for (int i = 0; i < mixer.outputDevices.Count; i++)
        {
            AudioOutputDevice device = CollectionUtils.GetDictionaryValueAtindex(mixer.outputDevices, i);
            AudioDeviceData deviceData = new AudioDeviceData(device);
            int index = i + mixer.inputDevices.Count;
            deviceDatas[index] = deviceData;
        }

        return deviceDatas;
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