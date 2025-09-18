using CSharpAlgorithms.Collections;

namespace CSharpAlgorithms.Audio;

public struct AudioDeviceData : IMute, IVolume
{
    public string Name { get; set; } = string.Empty;
    public bool IsInput { get; set; }
    public float Volume { get; set; } = 1;
    public bool IsMuted { get; set; } = false;

    public string[] Tags { get; set; } = [];

    public static readonly AudioDeviceData DefaultInput = new() {Name = "Default", IsInput = true};
    public static readonly AudioDeviceData DefaultOutput = new() {Name = "Default", IsInput = false};

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
}