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
using CSharpAlgorithms.Collection;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Audio;

/// <summary>
/// Represents an audio device that can be used for input and/or output. Allows reading from input buffers and writing to output buffers, as well as playing audio through the device using AudioPlayer instances. Manages an underlying PortAudio stream and handles audio processing in a callback function.
/// </summary>
public class AudioDevice : IMute, IDisposable
{
    public int DeviceIndex { get; private set; }
    public DeviceInfo Info { get; private set; }
    public bool IsMuted { get; set; } = false;
    public bool IsInputMuted, IsOutputMuted, IsPlayingInput;
    public int InputChannelCount { get; private set; }
    public int OutputChannelCount { get; private set; }

    public float InputVolume { get; private set; } = 1;
    public float OutputVolume { get; private set; } = 1;

    private List<BlockingCollection<AudioFrameCollection>> inputBuffers = [];
    private List<BlockingCollection<AudioFrameCollection>> outputBuffers = [];

    public List<AudioPlayer> audioPlayers = [];

    private AudioStream stream;

    public static Dictionary<string, AudioDevice> ActiveDevices { get; protected set; } = [];

    private AudioDevice(){}

    /// <summary>
    /// Gets a blocking buffer that this deivce will write audio frames into and consumers can read from.
    /// </summary>
    /// <returns></returns>
    public BlockingCollection<AudioFrameCollection> GetInputBuffer()
    {
        BlockingCollection<AudioFrameCollection> buffer = new BlockingCollection<AudioFrameCollection>(2);
        inputBuffers.Add(buffer);
        return buffer;
    }

    /// <summary>
    /// Gets a blocking buffer that this device reads audio frames from to write to the output.
    /// </summary>
    /// <returns></returns>
    public BlockingCollection<AudioFrameCollection> GetOutputBuffer()
    {
        BlockingCollection<AudioFrameCollection> buffer = new BlockingCollection<AudioFrameCollection>(2);
        outputBuffers.Add(buffer);
        return buffer;
    }

    public bool SetDevice(string deviceName)
    {
        if (!AudioUtils.GetDeviceIndex(deviceName, out int deviceIndex))
        {
            Debug.WriteErrorLine($"[AudioDevice.SetDevice] No audio device found with name {deviceName}");
            return false;
        }

        return SetDevice(deviceIndex);
    }         
    public bool SetDevice(int deviceIndex)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioDevice.SetDevice]";

        if (deviceIndex == PortAudio.NoDevice)
        {
            Debug.WriteErrorLine($"{CALL_PATH} No audio device found at index {deviceIndex}");
            return false;
        }

        PortAudio.Initialize();

        try
        {
            AudioStream previousStream = stream;

            Info = PortAudio.GetDeviceInfo(deviceIndex);
            Debug.PrintObject(Info);

            InputChannelCount = Calculator.Min(1, Info.maxInputChannels);
            OutputChannelCount = Calculator.Min(2, Info.maxOutputChannels);

            StreamParameters? inStreamParameters = null, outStreamParameters = null;
            if (InputChannelCount > 0)
            {
                inStreamParameters = new StreamParameters
                {
                    device = deviceIndex,
                    channelCount = InputChannelCount,
                    sampleFormat = SampleFormat.Float32,
                    suggestedLatency = Info.defaultLowInputLatency,
                    hostApiSpecificStreamInfo = IntPtr.Zero
                };  
            }

            if (OutputChannelCount > 0)
            {
                outStreamParameters = new StreamParameters
                {
                    device = deviceIndex,
                    channelCount = OutputChannelCount,
                    sampleFormat = SampleFormat.Float32,
                    suggestedLatency = Info.defaultLowOutputLatency,
                    hostApiSpecificStreamInfo = IntPtr.Zero
                };  
            }

            stream = new AudioStream(
                inParams: inStreamParameters,
                outParams: outStreamParameters,
                sampleRate: Info.defaultSampleRate,
                framesPerBuffer: 0,
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

            Debug.WriteSuccess($"{CALL_PATH} Set audio device to {Info.name} at index {deviceIndex}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.WriteErrorLine($"{CALL_PATH} Failed to set audio device to index {deviceIndex}");
            Debug.Print(ex);
            return false;
        }
    }

    public bool SetInputVolume(float volume)
    {
        if (volume < 0 || volume > 10)
        {
            Debug.WriteErrorLine($"[AudioDevice.SetInputVolume] Volume must be between 0 and 10. Given: {volume}");
            return false;
        }

        InputVolume = volume;
        return true;
    }

    public bool SetOutputVolume(float volume)
    {
        if (volume < 0 || volume > 10)
        {
            Debug.WriteErrorLine($"[AudioDevice.SetOutputVolume] Volume must be between 0 and 10. Given: {volume}");
            return false;
        }

        OutputVolume = volume;
        return true;
    }

    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioOutputDevice.Callback]";
#if DEBUG_MODE
        Console.WriteLine($"{CALL_PATH}: {this}");
        Console.WriteLine($"{CALL_PATH} ({Info.name}) Callback called");
#endif
        uint outputSampleCount = (frameCount * (uint)OutputChannelCount);

        float[] outputBuffer = new float[outputSampleCount];

        AudioFrameCollection outputFrames = new AudioFrameCollection(channelCount: OutputChannelCount, (int)frameCount);

        if (!IsInputMuted && input != IntPtr.Zero)
        {
            uint inputSampleCount = (frameCount * (uint)InputChannelCount);
            float[] inputSamplesBuffer = new float[inputSampleCount];
            Marshal.Copy(input, inputSamplesBuffer, 0, (int)inputSampleCount);

            Calculator.MultiplyNoNew(inputSamplesBuffer, InputVolume);

            AudioFrameCollection inputFrames = new AudioFrameCollection(inputSamplesBuffer, (uint)InputChannelCount);
            if (InputChannelCount == 1 && OutputChannelCount == 2)
                inputFrames.ToStereo();

            inputBuffers.ForEach(buffer => buffer.Add(inputFrames));

            if (IsPlayingInput)
                outputFrames.Add(inputFrames);
        }

        foreach (AudioPlayer player in audioPlayers)
        {
            if (!player.IsPlaying)
                continue;

            if (IsOutputMuted)
            {
                player.StepForward((int)frameCount);
                continue;
            }

            if (player.TryGetFrames(frameCount, out AudioFrameCollection playerFrames))
                outputFrames.Add(playerFrames);
        }

        foreach (var buffer in outputBuffers)
        {
            if (buffer.TryTake(out AudioFrameCollection frameCollection))
                outputFrames.Add(frameCollection);
        }

        Calculator.AddNoNew(outputBuffer, outputFrames.GetSamples());
        Calculator.MultiplyNoNew(outputBuffer, OutputVolume);


        return SendBuffer();

        StreamCallbackResult SendBuffer()
        {
            Marshal.Copy(outputBuffer, 0, output, outputBuffer.Length);
            return StreamCallbackResult.Continue;
        }
    }

    public override string ToString()
    {
        return $"""
        DeviceIndex: {DeviceIndex}
        Name: {Info.name}
        IsMuted: {IsMuted}
        IsInputMuted: {IsInputMuted}
        IsOutputMuted: {IsOutputMuted}
        IsPlayingInput: {IsPlayingInput}
        """;
    }

    public void Dispose()
    {
        stream?.Stop();
        stream?.Dispose();
    } 
        

    public static bool GetDevice(string name, out AudioDevice device)
    {
        if (ActiveDevices.ContainsKey(name))
        {
            device = ActiveDevices[name];
            return true;
        }

        device = new AudioDevice();
        if (device.SetDevice(name))
        {
            ActiveDevices[name] = device;
            return true;
        }

        return false;
    }

    public static string[] GetNamesOfUnusedDevices()
    {
        string[] names = AudioUtils.GetDeviceNames();
        string[] unusedNames = names.Where(name => !ActiveDevices.ContainsKey(name)).ToArray();
        return unusedNames;
    }

    public static bool GetRandomUnusedDeviceName(out string name)
    {
        string[] deviceNames = AudioUtils.GetDeviceNames();
        if (ActiveDevices.ContainsKey(deviceNames))
        {
            name = null;
            return false;
        }

        for (int i = 0; i < deviceNames.Length; i++)
        {
            if (!ActiveDevices.ContainsKey(deviceNames[i]))
            {
                name = deviceNames[i];
                return true;
            }
        }

        name = null;
        return false;
    } 
}