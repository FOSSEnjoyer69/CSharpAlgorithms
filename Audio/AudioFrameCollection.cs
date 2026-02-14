#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using CSharpAlgorithms.Collection;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Audio;

public class AudioFrameCollection
{
    protected AudioFrame[] frames;
    public int ChannelCount
    {
        get
        {
            if (frames is null)
                return 0;

            if (frames.First() is null)
                return 0;

            return frames.First().Samples.Length;
        }
    }
    public int Length
    {
        get
        {
            if (frames is null)
                return 0;

            return frames.Length;
        }
    }
    public int SampleCount => Length * ChannelCount;

    public AudioFrameCollection(int channelCount = 2, int? length = null)
    {
        frames = new AudioFrame[length ?? 0];
        for (int i = 0; i < length; i++)
        {
            float[] samples = new float[channelCount];
            frames[i] = new AudioFrame(samples);
        }
    }

    public AudioFrameCollection(AudioFrame[] frames) => SetFrames(frames);
    public AudioFrameCollection(float[] samples, uint channelCount) => SetSamples(samples, channelCount);

    public void Clamp(float min, float max)
    {
        foreach (var frame in frames)
            frame.Clamp(min, max);
    }

    public AudioFrame this[int index]
    {
        get => frames[index];
        set => frames[index] = value;
    }

    public AudioFrameCollection Sample(int position, int blockSize)
    {
        if (position < 0 || blockSize < 1 || position >= frames.Length)
            return new AudioFrameCollection(channelCount: 2, length: 0);

        int availableFrames = frames.Length - position;
        int framesToCopy = Calculator.Min(blockSize, availableFrames);
        AudioFrameCollection block = new AudioFrameCollection(channelCount: ChannelCount, framesToCopy);

        for (int i = 0; i < framesToCopy; i++)
            block[i] = frames[position + i];

        return block;
    }


    public float[] GetSamples()
    {
        if (Length <= 0)
            return [];

        float[] samples = new float[SampleCount];

        for (int frameIndex = 0; frameIndex < Length; frameIndex++)
        {
            AudioFrame frame = frames[frameIndex];

            for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
            {
                int sampleIndex = (frameIndex * ChannelCount) + channelIndex;
                samples[sampleIndex] = frame.Samples[channelIndex];
            }
        }

        return samples;
    }
    public void SetSamples(float[] samples, uint channelCount)
    {
        if (samples is null || samples.Length == 0 || channelCount <= 0)
            throw new ArgumentException("Invalid samples or channel count.");

        if (samples.Length % channelCount != 0)
            throw new Exception("Warning: sample count not divisible by channel count.");

        int frameCount = samples.Length / (int)channelCount;
        frames = new AudioFrame[frameCount];

        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
        {
            float[] frameSamples = new float[channelCount];

            for (int channelIndex = 0; channelIndex < channelCount; channelIndex++)
            {
                int sampleIndex = (frameIndex * (int)channelCount) + channelIndex;
                frameSamples[channelIndex] = samples[sampleIndex];
            }

            AudioFrame frame = new AudioFrame(frameSamples);
            frames[frameIndex] = frame;
        }
    }

    public void SetFrames(AudioFrame[] frames)
    {
        Array.Copy(frames, this.frames, frames.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="frames"></param>
    public void Add(AudioFrameCollection additionFrames)
    {
        if (additionFrames is null || additionFrames.Length == 0)
            return;

        int minLength = Calculator.Min(frames.Length, additionFrames.Length);

        for (int i = 0; i < minLength; i++)
        {
            AudioFrame frame = frames[i];
            AudioFrame additionFrame = additionFrames[i];

            frame.AddSamples(additionFrame);
        }
    }

    public void ToStereo()
    {
        if (frames is null || Length <= 0 || ChannelCount == 2)
            return;

        foreach (AudioFrame frame in frames)
        {
            if (!frame.IsMono)
                continue;

            float[] steroSamples = new float[2];
            float sample = frame.Samples.First();
            steroSamples[0] = sample;
            steroSamples[1] = sample;

            frame.Samples = steroSamples;
        }
    }

    public static implicit operator AudioFrame[](AudioFrameCollection collection) => [.. collection.frames];
    public static implicit operator AudioFrameCollection(AudioFrame[] frames) => new AudioFrameCollection(frames);

}