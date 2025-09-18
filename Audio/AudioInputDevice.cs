#pragma warning disable

using CSharpAlgorithms.Collections;
using CSharpAlgorithms.Interfaces;
using CSharpAlgorithms.UUID;

using PortAudioSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AudioStream = PortAudioSharp.Stream;

namespace CSharpAlgorithms.Audio;

public class AudioInputDevice : IReadMix, IMute, IVolume, IChannelCountByte, IDisposable
{
    public int DeviceIndex { get; private set; }
    public DeviceInfo Info { get; private set; }
    public bool IsMuted { get; set; } = false;
    public float Volume { get; set; } = 1;
    public byte ChannelCount { get; protected set; }

    public AudioFrameCollection Mix { get; private set; } = [];

    /// <summary>
    /// Stored reference for disposal
    /// </summary>
    private AudioStream stream;

    public AudioInputDevice(string name)
    {
        PortAudio.Initialize();
        SetInput(name);
    }

    public void SetInput(string deviceName) => SetInput(AudioUtils.GetDeviceIndex(deviceName));
    public void SetInput(int deviceIndex)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioInputDevice.SetInput]";

        if (deviceIndex == PortAudio.NoDevice)
        {
            Debug.WriteErrorLine($"{CALL_PATH} No audio device found at index {deviceIndex}");
            return;
        }

        
        AudioStream? previousStream = stream;
        int previousDeviceIndex = DeviceIndex;
        DeviceInfo previousInfo = Info;
        byte previousChannelCount = ChannelCount;

        try
        {
            Info = PortAudio.GetDeviceInfo(deviceIndex);
            ChannelCount = (byte)MathF.Min(2, Info.maxInputChannels);

            StreamParameters inputParams = new StreamParameters
            {
            device = deviceIndex,
            channelCount = ChannelCount,
            sampleFormat = SampleFormat.Float32,
            suggestedLatency = Info.defaultLowInputLatency,
            hostApiSpecificStreamInfo = IntPtr.Zero
            };

            // Use device default sample rate instead of forcing AudioSettings.SampleRate
            AudioStream newStream = new AudioStream
            (
            inParams: inputParams,
            outParams: null,
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

            newStream.Start();

            stream = newStream;
            DeviceIndex = deviceIndex;
        }
        catch
        {
            // Restore previous state
            Info = previousInfo;
            ChannelCount = previousChannelCount;
            DeviceIndex = previousDeviceIndex;
            stream = previousStream;
            throw;
        }
    }


    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
    {
        try
        {
            int sampleCount = (int)(frameCount * ChannelCount);
            float[] buffer = new float[sampleCount];
            Marshal.Copy(input, buffer, 0, sampleCount);
            AudioFrameCollection frames = new(buffer, ChannelCount);
            Mix = frames * (IsMuted ? 0 : Volume);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Input callback error: {ex.Message}");
        }

        return StreamCallbackResult.Continue;
    }

    public AudioFrameCollection ReadMix()
    {
        AudioFrameCollection mix = new AudioFrameCollection(Mix);
        return mix;
    }

    public void Dispose()
    {
        stream?.Dispose();
    }
}
