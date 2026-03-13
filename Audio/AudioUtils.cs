//#define DEBUG

using System;
using System.Linq;
using CSharpAlgorithms.Collection;
using PortAudioSharp;

namespace CSharpAlgorithms.Audio;

public static class AudioUtils
{
    public const double A4Note = 440.0;

    public static bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// Initializes the PortAudio library. This method should be called before using any other audio functions.
    /// </summary>
    /// <returns>True if initialization was successful or is is already initialized, false otherwise</returns>
    public static bool Init()
    {
        if (IsInitialized)
            return true;

        try
        {
            PortAudio.Initialize();

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                if (IsInitialized)
                {
                    PortAudio.Terminate();
                    IsInitialized = false;
                }
            };

            IsInitialized = true;
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteErrorLine($"Error initializing Audio: {ex.Message}");
            IsInitialized = false;
            return false;
        }
    }

    public static bool GetDeviceIndex(string name, out int deviceIndex)
    {
        DeviceInfo[] devices = GetDevices();
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name == name)
            {
                deviceIndex = i;
                return true;
            }           
        }

        deviceIndex = -1;
        return false;
    }

    public static DeviceInfo[] GetDevices()
    {
        Init();

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
        DeviceInfo[] devices = GetDevices();
        string[] names = devices.Select(d => d.name).ToArray();

        return names;
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

    public static bool IsDeviceAvailable(string deviceName)
    {
        if (!GetDeviceIndex(deviceName, out int index))
            return false;

        return IsDeviceAvailable(index);
    }
    public static bool IsDeviceAvailable(int deviceIndex)
    {
        Init();

        int count = PortAudio.DeviceCount;
        return deviceIndex >= 0 && deviceIndex < count;
    }

    public static void AddSamples(float[] source, float[] target)
    {
        float[] sourceCopy = new float[source.Length];
        source.CopyTo(sourceCopy, 0);

        if (target.Length == 2 && source.Length == 1)
            ArrayUtils.Stretch(sourceCopy, ratio: 2);

        for (int i = 0; i < target.Length; i++)
        {
            target[i] += source[i];
        }
    }
}