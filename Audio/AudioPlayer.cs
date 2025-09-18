#pragma warning disable

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using CSharpAlgorithms.Interfaces;

using PortAudioSharp;
using AudioStream = PortAudioSharp.Stream;


namespace CSharpAlgorithms.Audio;

using static Calculator;

public class AudioPlayer : IReadMix, IPlay, IPause, IIsPlaying
{
    public AudioClip clip;

    public int Position { get; set; } = 0;


    public bool IsPlaying { get; private set; } = false;
    public bool IsFinished => Position >= clip.Length;
    public bool Loop { get; set; } = false;

    private AudioFrameCollection m_buffer = new((int)AudioSettings.FramesPerBuffer);

    private AudioStream stream;

    public AudioPlayer(AudioClip clip)
    {
        this.clip = clip;

        int deviceIndex = AudioUtils.GetDeviceIndex("default");
        DeviceInfo Info = PortAudio.GetDeviceInfo(deviceIndex);

        StreamParameters streamParams = new StreamParameters
        {
            device = deviceIndex,
            channelCount = 2,
            sampleFormat = SampleFormat.Float32,
            suggestedLatency = Info.defaultLowInputLatency,
            hostApiSpecificStreamInfo = IntPtr.Zero
        };

        // Use device default sample rate instead of forcing AudioSettings.SampleRate
        stream = new AudioStream
        (
            inParams: null,
            outParams: streamParams,
            sampleRate: AudioSettings.SampleRate,
            framesPerBuffer: AudioSettings.FramesPerBuffer,
            streamFlags: StreamFlags.ClipOff,
            callback: Callback,
            userData: IntPtr.Zero
        );

        stream.Start();
    }

    public void Play() => IsPlaying = true;
    public void Pause() => IsPlaying = false;

    private AudioFrameCollection GetFrames(int frameCount)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioPlayer.GetFrames]";

        if (!IsPlaying)
            return [];

        int remainingFrames = clip.Length - Position;
        int framesToRead = Calculator.Min(frameCount, remainingFrames);

        AudioFrameCollection frames = clip.Frames.Sample(Position, framesToRead);

        Position += framesToRead;
        if (IsFinished)
        {
            Position = 0;

            if (!Loop)
                IsPlaying = false;
        }

        return frames;
    }

    private StreamCallbackResult Callback(IntPtr input, IntPtr output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
    {
        m_buffer = GetFrames((int)frameCount);
        return StreamCallbackResult.Continue;
    }

    public AudioFrameCollection ReadMix() => m_buffer;    
}