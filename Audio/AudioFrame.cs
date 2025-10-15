using CSharpAlgorithms.Interfaces;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Audio;

public class AudioFrame : IClamp<float>
{
    /// <summary>
    /// Samples for each channel
    /// </summary>
    public float[] Samples;
    public bool IsMono, IsStero;

    public AudioFrame(float[] samples)
    {
        if (samples is null)
            throw new NullReferenceException("Samples cannot be null");

        Samples = samples;

        IsMono = samples.Length == 1;
        IsStero = samples.Length == 2;
    }


    public void AddSamples(AudioFrame frame)
    {
        if (frame is null || frame.Samples is null || frame.Samples.Length == 0)
            return;

        int channelCount = Calculator.Min(Samples.Length, frame.Samples.Length);
        for (int i = 0; i < channelCount; i++)
        {
            Samples[i] += frame.Samples[i];
        }
    }
    public void Clamp(float min, float max)
    {
        if (Samples is null)
            return;

        for (int i = 0; i < Samples.Length; i++)
            Samples[i] = Calculator.ClampInclusive(Samples[i], min, max);
    }
}