#define USE_SOUND_FLOW

using PortAudioSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AudioStream = PortAudioSharp.Stream;
using CSharpAlgorithms.Collection;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Audio;

/// <summary>
/// Represents an audio device that can be used for input and/or output. Allows reading from input buffers and writing to output buffers, as well as playing audio through the device using AudioPlayer instances. Manages an underlying PortAudio stream and handles audio processing in a callback function.
/// </summary>
public sealed class AudioDevice : IMute, IDisposable
{
    public int DeviceIndex { get; private set; }
    public DeviceInfo Info { get; private set; }
    public bool IsMuted { get; set; } = false;
    public bool IsInputMuted, IsOutputMuted;
    public bool MonitorOwnInput;
    public int InputChannelCount { get; private set; }
    public int OutputChannelCount { get; private set; }

    public bool HasInput => InputChannelCount > 0;
    public bool HasOutput => OutputChannelCount > 0;

    public float InputVolume { get; private set; } = 1;
    public float OutputVolume { get; private set; } = 1;

    protected Dictionary<string, BlockingCollection<AudioFrameCollection>> inputBuffers = [];
    protected Dictionary<string, BlockingCollection<AudioFrameCollection>> outputBuffers = [];

    public List<AudioPlayer> audioPlayers = [];

    private AudioStream stream;

    public static Dictionary<string, AudioDevice> ActiveDevices { get; protected set; } = [];

    float[] inputMonitorBuffer = null!,
            inputSamplesBuffer = null!, 
            outputSamplesBuffer = null!;

    private AudioDevice() { }

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

        AudioUtils.Init();

        try
        {
            AudioStream previousStream = stream;
            DeviceInfo previousInfo = Info;

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

            if (previousStream is not null)
            {
                if (previousStream.IsActive)
                    previousStream.Stop();

                previousStream.Dispose();
            }

            stream = new AudioStream(
                inParams: inStreamParameters,
                outParams: outStreamParameters,
                sampleRate: Info.defaultSampleRate,
                framesPerBuffer: 64,
                streamFlags: StreamFlags.ClipOff,
                callback: Callback,
                userData: IntPtr.Zero
            );

            stream.Start();

            DeviceIndex = deviceIndex;

            foreach (KeyValuePair<string, BlockingCollection<AudioFrameCollection>> item in inputBuffers)
            {
                if (item.Key == previousInfo.name)
                {
                    outputBuffers[item.Key] = item.Value;
                    inputBuffers.Remove(item.Key);
                }
            }

            foreach (KeyValuePair<string, BlockingCollection<AudioFrameCollection>> item in outputBuffers)
            {
                if (item.Key == previousInfo.name)                
                {
                    inputBuffers[item.Key] = item.Value;
                    outputBuffers.Remove(item.Key);
                }
            }

            Debug.WriteSuccess($"{CALL_PATH} Set audio device to {Info.name} at index {deviceIndex}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.WriteErrorLine($"{CALL_PATH} Failed to set audio device to index '{deviceIndex}' with name '{Info.name}'");
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

        uint outputSampleCount = frameCount * (uint)OutputChannelCount;
        outputSamplesBuffer = new float[outputSampleCount];

        AudioFrameCollection outputFrames = new(channelCount: OutputChannelCount, (int)frameCount);

        if (input != IntPtr.Zero)
        {
            if (InputChannelCount > 0 && !IsInputMuted)
            {
                uint inputSampleCount = frameCount * (uint)InputChannelCount;
                inputSamplesBuffer = new float[inputSampleCount];
                Marshal.Copy(input, inputSamplesBuffer, 0, (int)inputSampleCount);

                Calculator.MultiplyNoNew(inputSamplesBuffer, InputVolume);

                AudioFrameCollection inputFrames = new(inputSamplesBuffer, (uint)InputChannelCount);
                if (InputChannelCount == 1 && OutputChannelCount == 2)
                    inputFrames.ToStereo();

                if (MonitorOwnInput)
                    inputMonitorBuffer = inputFrames.GetSamples();

                foreach (var buffer in inputBuffers.Values)
                    buffer.Add(inputFrames);
            }
        }

        if (output != IntPtr.Zero)
        {
            foreach (AudioPlayer player in audioPlayers)
            {
                if (IsOutputMuted)
                {
                    player.StepForward((int)frameCount);
                    continue;
                }

                if (player.TryGetFrames(frameCount, out AudioFrameCollection playerFrames))
                {
                    if (OutputChannelCount == 2 && playerFrames.ChannelCount == 1)
                        playerFrames.ToStereo();
                    
                    outputFrames.Add(playerFrames);
                }
            }

            foreach (var bufferDicItem in outputBuffers)
            {
                BlockingCollection<AudioFrameCollection> buffer = bufferDicItem.Value;

                if (buffer.TryTake(out AudioFrameCollection frameCollection))
                {
                    if (IsOutputMuted)
                        continue;

                    frameCollection.ToStereo();
                    outputFrames.Add(frameCollection);

                }
                else
                    Debug.WriteWarning($"{CALL_PATH} {bufferDicItem.Key} Output buffer for device {Info.name} is empty");
            }

            if (inputMonitorBuffer is not null && MonitorOwnInput && !IsOutputMuted)
                Calculator.AddNoNew(outputSamplesBuffer, inputMonitorBuffer);
                
            Calculator.AddNoNew(outputSamplesBuffer, outputFrames.GetSamples());
            Calculator.MultiplyNoNew(outputSamplesBuffer, OutputVolume);

            Marshal.Copy(outputSamplesBuffer, 0, output, outputSamplesBuffer.Length);
        }

        return StreamCallbackResult.Continue;
    }

    public override string ToString()
    {
        return $"""
        DeviceIndex: {DeviceIndex}
        Name: {Info.name}
        IsMuted: {IsMuted}
        IsInputMuted: {IsInputMuted}
        IsOutputMuted: {IsOutputMuted}
        MonitorOwnInput: {MonitorOwnInput}
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

    public static AudioDevice[] GetOtherActiveDevices(AudioDevice device)
    {
        return DictionaryUtils.GetValues(ActiveDevices).Where(d => d != device).ToArray();
    }

    public static bool LoadDevicesFromFile(string filePath = "")
    {
        AudioDeviceData[] deviceDatas = AudioDeviceData.Load(filePath);
        foreach (AudioDeviceData deviceData in deviceDatas)
        {
            if (!GetDevice(deviceData.Name, out AudioDevice device))
                continue;

            device.SetInputVolume(deviceData.InputVolume);
            device.SetOutputVolume(deviceData.OutputVolume);
            device.IsMuted = deviceData.IsMuted;
        }

        return true;
    }

    public static bool ConnectDevices(AudioDevice source, AudioDevice destination)
    {
        if (source is null || destination is null)
        {
#if true
            if (source is null && destination is null)
                Debug.WriteErrorLine("[AudioDevice.ConnectDevices] Both source and destination devices are null");
            else if (source is null)
                Debug.WriteErrorLine("[AudioDevice.ConnectDevices] Source device is null");
            else
                Debug.WriteErrorLine("[AudioDevice.ConnectDevices] Destination device is null");
#endif

            return false;
        }

        if (source == destination)
        {
#if true
            Debug.WriteErrorLine("[AudioDevice.ConnectDevices] Source and destination devices are the same");
#endif
            return false;
        }

        BlockingCollection<AudioFrameCollection> buffer = new BlockingCollection<AudioFrameCollection>(boundedCapacity: 64);
        source.inputBuffers[destination.Info.name] = buffer;
        destination.outputBuffers[source.Info.name] = buffer;

        Debug.WriteSuccess($"[AudioDevice.ConnectDevices] Created a connection between source device '{source.Info.name}' and destination device '{destination.Info.name}'");

        return true;
    }

    public static void DisconnectDevices(AudioDevice source, AudioDevice destination)
    {
        if (source is null || destination is null)
        {
            return;
        }

        if (source == destination)
            return;

        source.inputBuffers.Remove(destination.Info.name);
        destination.outputBuffers.Remove(source.Info.name);
    }
}