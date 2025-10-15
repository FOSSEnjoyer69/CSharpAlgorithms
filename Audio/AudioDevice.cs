#pragma warning disable
//#define DEBUG_MODE

using PortAudioSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using AudioStream = PortAudioSharp.Stream;

namespace CSharpAlgorithms.Audio;

public class AudioDevice : IMute, IVolume, IDisposable
{
    public int DeviceIndex { get; private set; }
    public DeviceInfo Info { get; private set; }
    public bool IsMuted { get; set; } = false;
    public float Volume { get; set; } = 1;
    public int InputChannelCount { get; protected set; }
    public int OutputChannelCount { get; protected set; }

    public BlockingCollection<AudioFrameCollection> inputFrameBuffer = new BlockingCollection<AudioFrameCollection>(2);

    public List<AudioDevice> inputDevices = [];
    private List<AudioPlayer> players = [];

    private AudioStream stream;

    public static Dictionary<string, AudioDevice> ActiveDevices { get; protected set; } = [];

    public AudioDevice(string name = "default")
    {
        PortAudio.Initialize();
        SetDevice(name);
    }

    public void SetDevice(string deviceName) => SetDevice(AudioUtils.GetDeviceIndex(deviceName));
    public void SetDevice(int deviceIndex)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioDevice.SetDevice]";

        if (deviceIndex == PortAudio.NoDevice)
        {
            Debug.WriteErrorLine($"{CALL_PATH} No audio device found at index {deviceIndex}");
            return;
        }

        AudioStream previousStream = stream;

        Info = PortAudio.GetDeviceInfo(deviceIndex);
        Debug.PrintObject(Info);

        InputChannelCount = 1;
        OutputChannelCount = 2;

        StreamParameters inStreamParameters = new StreamParameters
        {
            device = deviceIndex,
            channelCount = InputChannelCount,
            sampleFormat = SampleFormat.Float32,
            suggestedLatency = (AudioSettings.Latency / 1000),
            hostApiSpecificStreamInfo = IntPtr.Zero
        };


        StreamParameters outStreamParameters = new StreamParameters
        {
            device = deviceIndex,
            channelCount = OutputChannelCount,
            sampleFormat = SampleFormat.Float32,
            suggestedLatency = (AudioSettings.Latency / 1000),
            hostApiSpecificStreamInfo = IntPtr.Zero
        };

        stream = new AudioStream(
            inParams: inStreamParameters,
            outParams: outStreamParameters,
            sampleRate: AudioSettings.SampleRate,
            framesPerBuffer: AudioSettings.FramesPerBuffer,
            streamFlags: StreamFlags.ClipOff,
            callback: Callback,
            userData: IntPtr.Zero
        );


        if (previousStream is not null)
        {
            previousStream.Stop();
            previousStream.Dispose();
        }

        stream.Start();

        DeviceIndex = deviceIndex;
    }

    public void AddPlayer(AudioPlayer player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }


    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioOutputDevice.Callback]";
#if DEBUG_MODE
        Console.WriteLine($"{CALL_PATH} ({Info.name}) Callback called");
#endif
        uint sampleCount = (frameCount * (uint)OutputChannelCount);

        float[] inputSamplesBuffer = new float[sampleCount];
        Marshal.Copy(input, inputSamplesBuffer, 0, (int)sampleCount);
        AudioFrameCollection inputFrames = new AudioFrameCollection(inputSamplesBuffer, 2);
        inputFrameBuffer.TryAdd(inputFrames);

        float[] outputBuffer = new float[sampleCount];

        bool playInput = true;
        if (playInput && input != IntPtr.Zero)
        {
            Array.Copy(inputSamplesBuffer, outputBuffer, sampleCount);
        }

        AudioFrameCollection frames = new AudioFrameCollection(channelCount: 2, (int)frameCount);

        foreach (AudioDevice devices in inputDevices)
        {
            if(devices.inputFrameBuffer.TryTake(out AudioFrameCollection frameCollection))
            frames.Add(frameCollection);
        }

        foreach (AudioPlayer player in players)
        {
#if DEBUG_MODE
            Console.WriteLine($"{CALL_PATH} getting {frameCount} frames from {player.clip.Name}");
#endif

            AudioFrameCollection playerframes = player.GetFrames(frameCount);
            playerframes.ToStereo();
            frames.Add(playerframes);
        }

        outputBuffer = frames.GetSamples();

        return SendBuffer();

        StreamCallbackResult SendBuffer()
        {
            Marshal.Copy(outputBuffer, 0, output, outputBuffer.Length);
            return StreamCallbackResult.Continue;
        }
    }


    public void Dispose() => stream?.Dispose();

    public static AudioDevice CreateNewDevice(string name = "")
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioDevice.CreateNewDevice]";

        string[] unusedDeviceNames = AudioUtils.GetDeviceNames();
        foreach (var deviceName in ActiveDevices.Keys)
        {
            unusedDeviceNames = Array.FindAll(unusedDeviceNames, name => name != deviceName);
        }

        AudioDevice device  = new AudioDevice(name);
        

        ActiveDevices[device.Info.name] = device;
        Console.WriteLine($"{CALL_PATH} create new audio device: {device.Info.name}");

        return device;
    }

    public static AudioDevice GetDevice(string name="default")
    {
        if (ActiveDevices.ContainsKey(name))
            return ActiveDevices[name];

        AudioDevice device = CreateNewDevice(name);
        ActiveDevices[name] = device;
        return device;
    }
}