using System;
using System.Linq;
using System.Numerics;
using CSharpAlgorithms.Interfaces;

namespace CSharpAlgorithms.Audio;

public readonly struct AudioFrame : IAdditionOperators<AudioFrame, AudioFrame, AudioFrame>, IClamp<float, float>
{
    /// <summary>
    /// Channel Samples, '0' is left ,'1' is right and so on
    /// </summary>
    public readonly float[] Samples;
    public readonly bool IsMono, IsStero;

    public AudioFrame(float[] samples)
    {
        if (samples is null)
            throw new NullReferenceException("Samples cannot be null");

        Samples = samples;

        IsMono = samples.Length == 1;
        IsStero = samples.Length == 2;
    }

    public void Clamp(float min, float max)
    {
        if (Samples is null)
            return;

        for (int i = 0; i < Samples.Length; i++)
            Samples[i] = Calculator.ClampInclusive(Samples[i], min, max);
    }

    public static AudioFrame operator +(AudioFrame left, AudioFrame right)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioFrame.+]";

        float[] aSamples = left.Samples;
        float[] bSamples = right.Samples;

        if (aSamples is null && bSamples is null)
            throw new NullReferenceException($"{CALL_PATH} Cannot add AudioFrames with null Samples");

        if (aSamples is null)
            return right;
        
        if (bSamples is null)
            return left;

        int minChannelCount = Calculator.Min(aSamples.Length, bSamples.Length);
        float[] sumSamples = new float[minChannelCount];

        for (int i = 0; i < minChannelCount; i++)
            sumSamples[i] = aSamples[i] + bSamples[i];

        AudioFrame frame = new AudioFrame(sumSamples);
        return frame;
    }
}