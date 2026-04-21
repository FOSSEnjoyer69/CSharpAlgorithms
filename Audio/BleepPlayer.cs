using System;
using CSharpAlgorithms.Collection;
using CSharpAlgorithms.Interfaces;
using SysMath = System.Math;

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

    private const double TwoPi = SysMath.PI * 2.0;

    public BleepPlayer(float frequency = 1_000)
    {
        Frequency = frequency;
    }

    public AudioFrameCollection GetFrames(uint frameCount)
    {
        if (!IsPlaying)
            return new();

        AudioFrame[] frames = new AudioFrame[frameCount];
        for (int i = 0; i < frames.Length; i++)
        {
            float sample = (float)SysMath.Sin(phase);
            //float[] samples = CollectionUtils.Repeat(sample, channelCount);
            float[] samples = ArrayUtils.Repeat(sample, 2);
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
        CSDebug.WriteLine($"{CALL_PATH} playing");
    }
    public void Pause()
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.BleepPlayer.Pause]";
        IsPlaying = false;
        CSDebug.WriteLine($"{CALL_PATH} paused");
    }
}
