using System.Collections.Concurrent;

namespace CSharpAlgorithms.Audio;

public class AudioPlayer : IAudioBuffer, IAudioProvider
{
    public AudioClip clip;
    private Thread playThread;

    private double latencySeconds;
    private int framesPerBuffer;
    private float[] samples;
    private int position = 0;
    private double blockDurationSeconds;

    public BlockingCollection<float[]> AudioBuffer { get; set; }

    public bool IsFinished => !(position < samples.Length);
    public bool IsPlaying { get; private set; } = false;
    public bool HasStarted { get; private set; } = false;

    private readonly ManualResetEventSlim pauseEvent = new(true); // starts unpaused
    private readonly object lockObj = new();

    public AudioPlayer(AudioClip clip, AudioMixer mixer)
    {
        AudioBuffer = mixer.AudioBuffer ?? throw new Exception("Output device must provide a buffer");

        this.clip = clip;
        latencySeconds = mixer.OutputDevice.Info.defaultLowOutputLatency;
        framesPerBuffer = (int)(clip.SampleRate * latencySeconds);
        samples = clip.Samples;
        blockDurationSeconds = (double)framesPerBuffer / clip.SampleRate;
    }

    public AudioPlayer(AudioClip clip, AudioOutputDevice outputDevice)
    {
        AudioBuffer = outputDevice.AudioBuffer ?? throw new Exception("Output device must provide a buffer");

        Console.WriteLine($"Clip Sample Rate: {clip.SampleRate}");
        Console.WriteLine($"Playback Sample Rate: {outputDevice.Info.defaultSampleRate}");

        framesPerBuffer = (int)(clip.SampleRate * latencySeconds);

        this.clip = clip;
        latencySeconds = outputDevice.Info.defaultLowOutputLatency;
        framesPerBuffer = (int)(clip.SampleRate * latencySeconds);
        samples = clip.Samples;
        blockDurationSeconds = (double)framesPerBuffer / clip.SampleRate;

    }

    public void Play()
    {
        if (clip is null)
        {
            Console.WriteLine("Cannot start playing 'clip' is null");
            return;
        }

        if (IsPlaying)
                return;

        IsPlaying = true;
        HasStarted = true;

        playThread = new Thread(PlaybackLoop)
        {
            IsBackground = true
        };

        playThread.Start();
    }

    public void Pause() => pauseEvent.Reset();
    public void Resume() => pauseEvent.Set();

    public void Restart()
    {
        lock (lockObj)
        {
            position = 0;
            IsPlaying = true;
        }

        if (playThread is not null && playThread?.IsAlive == true)
        {
            Resume();
        }
        else
        {
            HasStarted = false;
            Play();
        }
    }

    private void PlaybackLoop()
    {
        try
        {
            while (true)
            {
                pauseEvent.Wait();

                float[] block;

                lock (lockObj)
                {
                    if (!IsPlaying || position >= samples.Length)
                        break;

                    block = GetSamples(framesPerBuffer);
                    AudioBuffer.Add(block);
                }

                Thread.Sleep(TimeSpan.FromSeconds(blockDurationSeconds));
            }
        }
        catch
        {
            Console.WriteLine($"Audio playback thread error");
            throw;
        }
    }

    public float[] GetSamples(int frameCount)
    {
        int remaining = samples.Length - position;
        if (remaining <= 0) return Array.Empty<float>();

        int blockSize = Math.Min(frameCount * clip.ChannelCount, remaining);

        float[] block = new float[blockSize];
        Array.Copy(samples, position, block, 0, blockSize);
        position += blockSize;

        return block;
    }
}