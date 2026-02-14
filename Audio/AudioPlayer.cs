#pragma warning disable

using System;
using System.Collections.Concurrent;
using CSharpAlgorithms.Interfaces;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Audio;

public class AudioPlayer : IPlay, IPause, IIsPlaying, IAudioProvider
{
    public AudioClip clip;

    public int Position { get; set; } = 0;

    public bool IsPlaying { get; private set; } = false;
    public bool IsFinished => Position >= clip.Length;
    public bool Loop { get; set; } = false;
    public event Action<string> OnStateChanged;

    public AudioPlayer(AudioClip clip)
    {
        this.clip = clip;
    }

    public void Play()
    {
        IsPlaying = true;
        OnStateChanged?.Invoke("play");
    }
    public void Pause()
    {
        IsPlaying = false;
        OnStateChanged?.Invoke("pause");
    }

    public AudioFrameCollection GetFrames(uint frameCount)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioPlayer.GetFrames]";
        if (!IsPlaying)
            return new AudioFrameCollection();
        
        int remainingFrames = clip.Length - Position;
        if (remainingFrames <= 0)
        {
            Position = 0;
            if (!Loop)
            {
                IsPlaying = false;
                OnStateChanged?.Invoke("end");
                return new AudioFrameCollection();
            }
        }

        int framesToRead = Calculator.Min((int)frameCount, remainingFrames);
        AudioFrameCollection frames = clip.Frames.Sample(Position, framesToRead);

        StepForward(framesToRead);

        return frames;
    }

    public bool TryGetFrames(uint frameCount, out AudioFrameCollection frames)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioPlayer.GetFrames]";
        if (frameCount == 0 || !IsPlaying)
        {
            frames = null;
            return false;
        }

        int remainingFrames = clip.Length - Position;
        if (remainingFrames <= 0)
        {
            Position = 0;
            if (!Loop)
            {
                IsPlaying = false;
                OnStateChanged?.Invoke("end");
            }

            frames = null;
            return false;
        }

        int framesToRead = Calculator.Min((int)frameCount, remainingFrames);
        if (framesToRead <= 0)
        {
            frames = null;
            return false;
        }

        frames = clip.Frames.Sample(Position, framesToRead);
        StepForward(framesToRead);

        return true;
    }

    public void StepForward(int frameCount)
    {
        Position = Calculator.Min(Position + frameCount, clip.Length);
    }
}