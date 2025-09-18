using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CSharpAlgorithms.Audio;

public class AudioFrameCollection : ICollection<AudioFrame>, IAdditionOperators<AudioFrameCollection, AudioFrameCollection, AudioFrameCollection>
{
    protected List<AudioFrame> frames;
    public readonly ushort MaxChannelCount = 2;


    public int Count => frames.Count;
    public bool IsReadOnly => false;

    public AudioFrameCollection(int capacity = -1)
    {
        if (capacity < 0)
            capacity = (int)AudioSettings.FramesPerBuffer;

        frames = new List<AudioFrame>(capacity);
    }
    public AudioFrameCollection(AudioFrame[] frames) => this.frames = [.. frames];

    public AudioFrameCollection(float[] samples, short channelCount)
    {
        if (samples is null || samples.Length == 0 || channelCount == 0)
            return;

        int frameCount = samples.Length / channelCount;
        AudioFrame[] frames = new AudioFrame[frameCount];

        for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
        {
            float[] frameSamples = new float[channelCount];

            for (int channelIndex = 0; channelIndex < channelCount; channelIndex++)
            {
                int sampleIndex = frameIndex * channelCount + channelIndex;
                frameSamples[channelIndex] = samples[sampleIndex];
            }

            frames[frameIndex] = new AudioFrame(frameSamples);
        }

        this.frames = [.. frames];
        MaxChannelCount = (ushort)channelCount;
    }

    public void Clamp(float min, float max)
    {
        foreach (var frame in frames)
            frame.Clamp(min, max);
    }

    public AudioFrameCollection Sample(int position, int blockSize)
    {
        if (position < 0 || blockSize < 1 || position >= frames.Count)
            return new AudioFrameCollection();

        int availableFrames = frames.Count - position;
        int framesToCopy = Math.Min(blockSize, availableFrames);
        AudioFrameCollection block = new AudioFrameCollection(framesToCopy);

        for (int i = 0; i < framesToCopy; i++)
            block.Add(frames[position + i]);

        return block;
    }

    public void Add(AudioFrame item) => frames.Add(item);
    public bool Remove(AudioFrame item) => frames.Remove(item);
    public void Clear() => frames.Clear();
    public bool Contains(AudioFrame item) => frames.Contains(item);
    public void CopyTo(AudioFrame[] array, int arrayIndex) => frames.CopyTo(array, arrayIndex);
    public IEnumerator<AudioFrame> GetEnumerator() => frames.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static AudioFrameCollection operator +(AudioFrameCollection left, AudioFrameCollection right)
    {
        int maxCount = Calculator.Max(left.Count, right.Count);
        AudioFrameCollection newCollection = new AudioFrameCollection(maxCount);

        Span<AudioFrame> aFrames = CollectionsMarshal.AsSpan(left.frames);
        Span<AudioFrame> bFrames = CollectionsMarshal.AsSpan(right.frames);

        for (int i = 0; i < maxCount; i++)
        {
            if (i < left.Count && i < right.Count)
                newCollection.Add(aFrames[i] + bFrames[i]);
            else if (i < left.Count)
                newCollection.Add(aFrames[i]);
            else if (i < right.Count)
                newCollection.Add(bFrames[i]);
        }

        return newCollection;
    }

    public static AudioFrameCollection operator *(AudioFrameCollection collection, float scalar)
    {
        AudioFrameCollection newCollection = new AudioFrameCollection(collection.Count);
        Span<AudioFrame> frames = CollectionsMarshal.AsSpan(collection.frames);

        for (int i = 0; i < collection.Count; i++)
        {
            AudioFrame frame = frames[i];
            float[] samples = frame.Samples;

            for (int j = 0; j < samples.Length; j++)
                samples[j] *= scalar;

            newCollection.Add(new AudioFrame(samples));
        }

        return newCollection;
    }

    public static implicit operator float[](AudioFrameCollection collection)
    {
        int sampleCount = collection.Count * collection.MaxChannelCount;
        float[] samples = new float[sampleCount];

        for (int frameIndex = 0; frameIndex < collection.Count; frameIndex++)
        {
            AudioFrame frame = collection.frames[frameIndex];
            float[] frameSamples = frame.Samples;

            for (int channelIndex = 0; channelIndex < frameSamples.Length; channelIndex++)
            {
                int sampleIndex = (frameIndex * collection.MaxChannelCount) + channelIndex;
                samples[sampleIndex] = frameSamples[channelIndex];
            }
        }

        return samples;
    }
    public static implicit operator AudioFrame[](AudioFrameCollection collection) => [.. collection.frames];
    public static implicit operator AudioFrameCollection(AudioFrame[] frames) => new AudioFrameCollection(frames);
    
}