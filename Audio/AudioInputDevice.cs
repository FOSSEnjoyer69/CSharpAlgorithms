#pragma warning disable

using PortAudioSharp;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using AudioStream = PortAudioSharp.Stream;

namespace CSharpAlgorithms.Audio;

public class AudioInputDevice : IAudioProvider, IDisposable
{
    public int DeviceIndex { get; private set; }
    public DeviceInfo Info { get; private set; }
    public BlockingCollection<float[]> AudioBuffer { get; private set; }
    private float[]? lastSamples = null;
    private int lastIndex = 0;

    private AudioStream stream;
    private int sampleRate;
    private int channelCount = 2;

    public AudioInputDevice(BlockingCollection<float[]>? audioBuffer = null)
    {
        PortAudio.Initialize();

        SetBuffer(audioBuffer);
        SetInput(PortAudio.DefaultInputDevice);
    }

    public void SetBuffer(BlockingCollection<float[]>? audioBuffer = null)
    {
        AudioBuffer = audioBuffer ?? new BlockingCollection<float[]>(boundedCapacity: 64);
    }

    public void SetInput(int deviceIndex)
    {
        if (deviceIndex == PortAudio.NoDevice)
            throw new InvalidOperationException($"No audio input device found at index {deviceIndex}.");

        try
        {
            AudioStream? previousStream = stream;

            DeviceInfo deviceInfo = PortAudio.GetDeviceInfo(deviceIndex);
            sampleRate = (int)deviceInfo.defaultSampleRate;

            StreamParameters inputParams = new StreamParameters
            {
                device = deviceIndex,
                channelCount = channelCount,
                sampleFormat = SampleFormat.Float32,
                suggestedLatency = deviceInfo.defaultLowInputLatency,
                hostApiSpecificStreamInfo = IntPtr.Zero
            };

            stream = new AudioStream(
                inParams: inputParams,
                outParams: null,
                sampleRate: sampleRate,
                framesPerBuffer: 0,
                streamFlags: StreamFlags.ClipOff,
                callback: Callback,
                userData: IntPtr.Zero
            );

            stream.Start();

            if (previousStream is not null)
            {
                previousStream.Stop();
                previousStream.Dispose();
            }

            DeviceIndex = deviceIndex;
            Info = deviceInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to set AudioInputDevice input: {ex.Message}");
            throw;
        }
    }

    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount,
        ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
    {
        int sampleCount = (int)(frameCount * channelCount);
        float[] buffer = new float[sampleCount];

        try
        {
            Marshal.Copy(input, buffer, 0, sampleCount);
            AudioBuffer.Add(buffer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Input callback error: {ex.Message}");
        }

        return StreamCallbackResult.Continue;
    }


    public float[] GetSamples(int frameCount, int channelCount)
    {
        int expectedSamples = frameCount * channelCount;
        float[] result = new float[expectedSamples];
        int written = 0;

        while (written < expectedSamples && (lastSamples != null || AudioBuffer.Count > 0))
        {
            if (lastSamples == null && AudioBuffer.TryTake(out var next))
            {
                lastSamples = next;
                lastIndex = 0;
            }

            if (lastSamples == null) break;

            int remaining = lastSamples.Length - lastIndex;
            int toCopy = Math.Min(remaining, expectedSamples - written);

            Array.Copy(lastSamples, lastIndex, result, written, toCopy);
            written += toCopy;
            lastIndex += toCopy;

            if (lastIndex >= lastSamples.Length)
            {
                lastSamples = null;
                lastIndex = 0;
            }
        }

        // Zero-fill if not enough samples
        if (written < expectedSamples)
        {
            Array.Clear(result, written, expectedSamples - written);
        }

        return result;
    }


    public void Dispose()
    {
        stream?.Dispose();
        PortAudio.Terminate();
    }
}
