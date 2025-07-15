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
}