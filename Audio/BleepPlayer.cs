using System;
using CSharpAlgorithms.Collections;

namespace CSharpAlgorithms.Audio;

public class BleepPlayer : IAudioProvider, IIsPlaying, IPlay, IPause
{
    private double phase = 0.0;
    private double increment;

    public float Frequency
    {
        get => (float)(increment * SampleRate / TwoPi);
        set => increment = (TwoPi * value) / SampleRate;
    }

    public uint SampleRate { get; private set; } = AudioSettings.SampleRate;
    public bool IsPlaying { get; private set; }

    private const double TwoPi = Math.PI * 2.0;

    public BleepPlayer(float frequency = 1_000)
    {
        Frequency = frequency;
    }

    public AudioFrameCollection GetFrames(uint frameCount, uint channelCount, GetSamplesArgs? args = null)
    {
        if (!IsPlaying)
            return [];

        AudioFrame[] frames = new AudioFrame[frameCount];
        for (int i = 0; i < frames.Length; i++)
        {
            float sample = (float)Math.Sin(phase);
            float[] samples = CollectionUtils.Repeat(sample, channelCount);
            AudioFrame frame = new AudioFrame(samples);
            frames[i] = frame;


            phase += increment;
            if (phase >= TwoPi)
                phase -= TwoPi;
        }

        return frames;
    }

    public void Play()
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.BleepPlayer.Play]";
        IsPlaying = true;
        Debug.WriteLine($"{CALL_PATH} playing");
    }
    public void Pause()
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.BleepPlayer.Pause]";
        IsPlaying = false;
        Debug.WriteLine($"{CALL_PATH} paused");
    }
}
