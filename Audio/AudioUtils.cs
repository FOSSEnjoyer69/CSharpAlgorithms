using System;
using System.Collections.Generic;
using System.Linq;
using CSharpAlgorithms.Collections;
using PortAudioSharp; 

namespace CSharpAlgorithms.Audio;

public static class AudioUtils
{
    public const double A4Note = 440.0;

    /// <summary>
    /// Gets index of device with name
    /// </summary>
    /// <param name="name">Name of device</param>
    /// <returns>Index of device or -1 if none found</returns>
    public static int GetDeviceIndex(string name)
    {
        DeviceInfo[] devices = GetDevices();
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name == name)
                return i;
        }

        return -1;
    }

    public static DeviceInfo[] GetDevices()
    {
        PortAudio.Initialize();

        int count = PortAudio.DeviceCount;
        if (count < 1)
            throw new Exception($"PortAudio.DeviceCount is less than 1, it is {count}");

        DeviceInfo[] devices = new DeviceInfo[count];

        for (int i = 0; i < count; i++)
            devices[i] = PortAudio.GetDeviceInfo(i);

        return devices;
    }

    public static string[] GetDeviceNames()
    {
        string[] inputNames = GetInputDeviceNames();
        string[] outputNames = GetOutputDeviceNames();
        string[] names = CollectionUtils.CombineArray([inputNames, outputNames]);

        return names;
    }
    public static string[] GetInputDeviceNames()
    {
        List<string> names = new List<string>();
        DeviceInfo[] devices = GetDevices();

        for (int i = 0; i < devices.Length; i++)
        {
            DeviceInfo device = devices[i];

            if (device.maxInputChannels > 0)
                names.Add(device.name);
        }

        return [.. names];
    }
    public static string[] GetOutputDeviceNames()
    {
        List<string> names = new List<string>();
        DeviceInfo[] devices = GetDevices();

        for (int i = 0; i < devices.Length; i++)
        {
            DeviceInfo device = devices[i];

            if (device.maxOutputChannels > 0)
                names.Add(device.name);
        }

        return [.. names];
    }

    public static string[] GetUnusedInputAudioDeviceNames(AudioInputDevice[] devices)
    {
        List<string> names = [.. GetInputDeviceNames()];

        foreach (AudioInputDevice device in devices)
            names.Remove(device.Info.name);

        return [.. names];
    }

    public static float[] MonoToStereo(float[] monoSamples)
    {
        float[] stereo = new float[monoSamples.Length * 2];
        for (int i = 0; i < monoSamples.Length; i++)
        {
            stereo[i * 2] = monoSamples[i];     // Left
            stereo[i * 2 + 1] = monoSamples[i]; // Right
        }
        return stereo;
    }

    public static string FormatDeviceNamesAsJSon()
    {
        var obj = new
        {
            inputDevices = GetInputDeviceNames(),
            outputDevices = GetOutputDeviceNames()
        };

        string json = System.Text.Json.JsonSerializer.Serialize(obj);
        return json;
    }

    public static string FormatAsJson(AudioDeviceData[] datas)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(datas);
        return json;
    }

    public static uint GetBufferSize(double seconds, uint sampleRate, ushort channelCount)
    {
        return (uint)(seconds * sampleRate * channelCount);
    }

    public static AudioFrame[] Resmaple(AudioFrame[] frames, byte channelCount)
    {
        byte originalChannel = (byte)frames.First().Samples.Length;

        if (originalChannel == 1 && channelCount == 2)
        {
            return ResampleMonoToStereo(frames);
        }

        return [];
    }

    public static AudioFrame[] ResampleMonoToStereo(AudioFrame[] frames)
    {
        AudioFrame[] newFrames = new AudioFrame[frames.Length];

        for (int i = 0; i < newFrames.Length; i++)
        {
            float sample = frames[i].Samples[0];
            float[] stereoSamples = [sample, sample];
            AudioFrame frame = new AudioFrame(stereoSamples);
            newFrames[i] = frame;
        }

        return newFrames;
    }
}