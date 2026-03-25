#pragma warning disable

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CSharpAlgorithms.Interfaces;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Audio;

public class AudioPlayer : IPlay, IPause, IIsPlaying, IAudioProvider, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private AudioClip m_clip;
    public AudioClip clip
    {
        get => m_clip;
        set
        {
            if (ReferenceEquals(m_clip, value))
                return;

            m_clip = value;
            OnPropertyChanged();
        }
    }

    private int m_position;
    public int Position
    {
        get => m_position;
        set
        {
            if (m_position == value)
                return;
            
            m_position = value;
            OnPropertyChanged();
        }
    }

    private bool m_isPlaying;
    public bool IsPlaying
    {
        get => m_isPlaying;
        set
        {
            if (m_isPlaying == value)
                return;

            m_isPlaying = value;
            OnPropertyChanged();
        }
    }

    private bool m_loop;
    public bool Loop
    {
        get => m_loop;
        set
        {
            if (m_loop == value)
                return;

            m_loop = value;
            OnPropertyChanged();
        }
    }

    public event Action<string> OnStateChanged;
    
    public AudioPlayer(AudioClip clip)
    {
        this.clip = clip;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    public void Stop()
    {
        IsPlaying = false;
        Position = 0;
        OnStateChanged?.Invoke("stop");
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
                OnStateChanged?.Invoke("stop");
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
                Stop();

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
        if (!IsPlaying)
            return;
            
        Position = Calculator.Min(Position + frameCount, clip.Length);
    }
}