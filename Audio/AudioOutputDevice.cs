using PortAudioSharp;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using AudioStream = PortAudioSharp.Stream;

namespace CSharpAlgorithms.Audio;

public class AudioOutputDevice : IDisposable
{
    public int DeviceIndex { get; private set; }

    public DeviceInfo Info { get; private set; }
    public BlockingCollection<float[]> AudioBuffer { get; set; }

    private float[]? lastSamples = null;
    private int lastIndex = 0;

    private AudioStream stream;
    private int sampleRate;

    private IAudioProvider audioProvider;

    public AudioOutputDevice(BlockingCollection<float[]>? audioBuffer = null)
    {
        PortAudio.Initialize();

        SetBuffer(audioBuffer);
        SetOutput(PortAudio.DefaultOutputDevice);
    }

    public void SetBuffer(BlockingCollection<float[]>? audioBuffer = null)
    {
        audioBuffer ??= [];
        AudioBuffer = audioBuffer;
    }

    public void SetAudioProvider(IAudioProvider provider)
    {
        audioProvider = provider;
    }

    public void SetOutput(int deviceIndex)
    {
        if (deviceIndex == PortAudio.NoDevice)
            throw new InvalidOperationException($"No audio output device found at {deviceIndex}.");

        try
        {
            AudioStream previousStream = stream;

            DeviceInfo deviceInfo = PortAudio.GetDeviceInfo(deviceIndex);


            sampleRate = (int)deviceInfo.defaultSampleRate;

            StreamParameters streamParameters = new StreamParameters
            {
                device = deviceIndex,
                channelCount = 2,
                sampleFormat = SampleFormat.Float32,
                suggestedLatency = deviceInfo.defaultLowOutputLatency,
                hostApiSpecificStreamInfo = IntPtr.Zero
            };

            stream = new AudioStream(
                inParams: null,
                outParams: streamParameters,
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
                lastSamples = null;
                lastIndex = 0;
            }

            DeviceIndex = deviceIndex;
            Info = deviceInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to set AudioOutputDevice output, {ex.Message}");
            throw;
        }
    }

    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount,
    ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
{
    int channelCount = 2;
    int expectedSamples = (int)(frameCount * channelCount);
    float[] buffer = new float[expectedSamples];

    if (audioProvider != null)
    {
        try
        {
            float[] samples = audioProvider.GetSamples((int)frameCount, channelCount);
            if (samples.Length < expectedSamples)
            {
                Array.Copy(samples, buffer, samples.Length);
                Array.Clear(buffer, samples.Length, expectedSamples - samples.Length);
            }
            else
            {
                Array.Copy(samples, buffer, expectedSamples);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AudioProvider error: {ex.Message}");
            Array.Clear(buffer, 0, expectedSamples); // Fallback to silence
        }
    }
    else
    {
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
            int toWrite = Math.Min(remaining, expectedSamples - written);

            Array.Copy(lastSamples, lastIndex, buffer, written, toWrite);
            written += toWrite;
            lastIndex += toWrite;

            if (lastIndex >= lastSamples.Length)
            {
                lastSamples = null;
                lastIndex = 0;
            }
        }

        if (written < expectedSamples)
        {
            Array.Clear(buffer, written, expectedSamples - written);
        }

        if (AudioBuffer.IsCompleted && lastSamples == null && lastIndex == 0)
        {
            return StreamCallbackResult.Complete;
        }
    }

    Marshal.Copy(buffer, 0, output, expectedSamples);
    return StreamCallbackResult.Continue;
}


    public void Dispose()
    {
        stream?.Dispose();
        PortAudio.Terminate();
    }
}

/*

    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount,
        ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
    {
        int channelCount = 2;
        int expectedSamples = (int)(frameCount * channelCount);
        float[] buffer = new float[expectedSamples];
        int written = 0;

        // Try to prefill AudioBuffer from IAudioProvider
        if (audioProvider != null)
        {
            try
            {
                float[] providerSamples = audioProvider.GetSamples((int)frameCount);
                if (providerSamples.Length > 0)
                {
                    AudioBuffer.Add(providerSamples);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AudioProvider error: {ex.Message}");
            }
        }

        while (written < expectedSamples && (lastSamples != null || AudioBuffer.Count > 0))
        {
            if (lastSamples == null && AudioBuffer.TryTake(out var next))
            {
                lastSamples = next;
                lastIndex = 0;
            }

            if (lastSamples == null) break;

            int remaining = lastSamples.Length - lastIndex;
            int toWrite = Math.Min(remaining, expectedSamples - written);

            Array.Copy(lastSamples, lastIndex, buffer, written, toWrite);
            written += toWrite;
            lastIndex += toWrite;

            if (lastIndex >= lastSamples.Length)
            {
                lastSamples = null;
                lastIndex = 0;
            }
        }

        // Fill remaining with silence
        if (written < expectedSamples)
        {
            Array.Clear(buffer, written, expectedSamples - written);
        }

        Marshal.Copy(buffer, 0, output, expectedSamples);

        if (AudioBuffer.IsCompleted && lastSamples == null && lastIndex == 0)
        {
            return StreamCallbackResult.Complete;
        }

        return StreamCallbackResult.Continue;
    }
*/