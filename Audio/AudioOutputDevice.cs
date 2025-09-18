#pragma warning disable

using CSharpAlgorithms.Interfaces;
using PortAudioSharp;
using System;
using System.Runtime.InteropServices;
using AudioStream = PortAudioSharp.Stream;

namespace CSharpAlgorithms.Audio;

public class AudioOutputDevice : IMute, IVolume, IChannelCountByte, IWriteToMix, IDisposable
{
    public int DeviceIndex { get; private set; }
    public DeviceInfo Info { get; private set; }
    public bool IsMuted { get; set; } = false;
    public float Volume { get; set; } = 1;
    public byte ChannelCount { get; protected set; } = 2;

    public AudioFrameCollection Mix { get; private set; } = [];

    private AudioStream stream;
    private IAudioProvider audioProvider;

    public AudioOutputDevice(string name = "default", IAudioProvider? provider=null)
    {
        PortAudio.Initialize();
        audioProvider = provider;
        SetOutput(name);
    }

    public void SetOutput(string deviceName) => SetOutput(AudioUtils.GetDeviceIndex(deviceName));
    public void SetOutput(int deviceIndex)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioOutputDevice.SetOutput]";

        if (deviceIndex == PortAudio.NoDevice)
        {
            Debug.WriteErrorLine($"{CALL_PATH} No audio device found at index {deviceIndex}");
            return;
        }

        AudioStream previousStream = stream;

        Info = PortAudio.GetDeviceInfo(deviceIndex);

        StreamParameters streamParameters = new StreamParameters
        {
            device = deviceIndex,
            channelCount = ChannelCount,
            sampleFormat = SampleFormat.Float32,
            suggestedLatency = Info.defaultLowOutputLatency,
            hostApiSpecificStreamInfo = IntPtr.Zero
        };

        stream = new AudioStream(
            inParams: null,
            outParams: streamParameters,
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

    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioOutputDevice.Callback]";
        float[] buffer = audioProvider.GetFrames(frameCount, ChannelCount);

        return SendBuffer();

        StreamCallbackResult SendBuffer()
        {
            Marshal.Copy(buffer, 0, output, buffer.Length);
            return StreamCallbackResult.Continue;
        }
    }

    public void Dispose()
    {
        stream?.Dispose();
    }

    public void WriteToMix(AudioFrame[] frames)
    {
        Mix = frames;
    }
}