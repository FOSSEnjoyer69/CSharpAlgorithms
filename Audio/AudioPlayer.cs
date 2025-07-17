using System;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace CSharpAlgorithms.Audio;

public class AudioPlayer : IAudioBuffer, IAudioProvider
{
    public AudioClip clip;
    public bool loop = false;

    private Thread playThread;



    private double latencySeconds;
    private int framesPerBuffer;
    private int position = 0;
    private double blockDurationSeconds;

    public BlockingCollection<float[]> AudioBuffer { get; set; }

    public bool IsFinished => !(position < clip.Samples.Length);
    public bool IsPlaying { get; private set; } = false;
    public bool HasStarted { get; private set; } = false;

    private readonly ManualResetEventSlim pauseEvent = new(true); // starts unpaused
    private readonly object lockObj = new();

    private readonly Mode mode;

    public enum Mode
    {
        WithOutputDevice,
        WithAudioMixer
    }

    public AudioPlayer(AudioClip clip, AudioOutputDevice outputDevice)
    {
        AudioBuffer = outputDevice.AudioBuffer ?? throw new Exception("Output device must provide a buffer");

        this.clip = clip;
        latencySeconds = outputDevice.Info.defaultLowOutputLatency;
        framesPerBuffer = (int)(clip.SampleRate * latencySeconds);
        blockDurationSeconds = (double)framesPerBuffer / clip.SampleRate;

        mode = Mode.WithOutputDevice;
    }

    public AudioPlayer(AudioClip clip, AudioMixer audioMixer)
    {
        this.clip = clip;

        audioMixer.sources.Add(this);

        mode = Mode.WithAudioMixer;
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

        Console.WriteLine($"Playing {clip.Name}");

        IsPlaying = true;
        HasStarted = true;

        if (mode == Mode.WithAudioMixer)
        {
            // No need to start a thread for AudioMixer, it will be handled by the mixer
            return;
        }

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
                    if (!IsPlaying || position >= clip.Samples.Length)
                        break;

                    block = GetSamples(framesPerBuffer, clip.ChannelCount);
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
    public float[] GetSamples(int frameCount, int channelCount)
    {
        if (!IsPlaying)
            return [];

        int clipChannels = clip.ChannelCount;
        int remaining = clip.Samples.Length - position;
        int samplesNeeded = frameCount * clipChannels;

        if (remaining <= 0)
        {
            position = 0;
            if (!loop)
                IsPlaying = false;

            return [];
        }

        int blockSize = Math.Min(samplesNeeded, remaining);
        float[] sourceBlock = new float[blockSize];
        Array.Copy(clip.Samples, position, sourceBlock, 0, blockSize);
        position += blockSize;

        // Upmix if needed
        if (clipChannels == channelCount)
        {
            return sourceBlock;
        }
        else if (clipChannels == 1 && channelCount == 2)
        {
            int frames = blockSize;
            float[] stereo = new float[frames * 2];

            for (int i = 0; i < frames; i++)
            {
                float monoSample = sourceBlock[i];
                stereo[i * 2] = monoSample;     // Left
                stereo[i * 2 + 1] = monoSample; // Right
            }

            return stereo;
        }
        else
        {
            throw new NotSupportedException("Unsupported channel conversion.");
        }
    }
}