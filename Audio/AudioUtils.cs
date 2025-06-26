using SoundIOSharp; 

namespace CSharpAlgorithms.Audio;

public static class AudioUtils
{
    public static SoundIO SoundIO;

    private static bool m_Initialized;

    public const double A4Note = 440.0;

    public static void Initialize()
    {
        if (m_Initialized)
            return;

        SoundIO = new SoundIO();
        SoundIO.Connect();
        SoundIO.FlushEvents();
        m_Initialized = true;
    }


    public static unsafe void WriteSampleS32NE(IntPtr ptr, double sample)
    {
        int* buffer = (int*)ptr;
        double range = (double)int.MaxValue - (double)int.MinValue;
        double val = sample * range / 2.0;
        *buffer = (int)val;
    }

    public static string[] GetInputDeviceNames()
    {
        Initialize();

        SoundIODevice[] devices = GetInputDevices();
        string[] names = devices.Select(device => device.Name).ToArray();

        return names;
    }
    public static SoundIODevice[] GetInputDevices()
    {
        Initialize();

        int deviceCount = SoundIO.GetInputDeviceCount();
        SoundIODevice[] devices = new SoundIODevice[deviceCount];

        for (int i = 0; i < deviceCount; i++)
        {
            SoundIODevice device = SoundIO.GetInputDevice(i);
            devices[i] = device;
        }

        return devices;
    }

    public static string[] GetOutputDeviceNames()
    {
        Initialize();

        SoundIODevice[] devices = GetOutputDevices();
        string[] names = devices.Select(device => device.Name).ToArray();

        return names;
    }

    public static SoundIODevice[] GetOutputDevices()
    {
        Initialize();

        int deviceCount = SoundIO.GetOutputDeviceCount();
        SoundIODevice[] devices = new SoundIODevice[deviceCount];

        for (int i = 0; i < deviceCount; i++)
        {
            SoundIODevice device = SoundIO.GetOutputDevice(i);
            devices[i] = device;
        }

        return devices;
    }
}