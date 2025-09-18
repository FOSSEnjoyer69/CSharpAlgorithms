#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpAlgorithms.Interfaces;


namespace CSharpAlgorithms.Audio;

public class AudioMixer : IAudioProvider
{
    public BleepPlayer BleepPlayer { get; set; }

    public bool PlayBleep { get; set; }

    public Dictionary<string, AudioInputDevice> inputDevices = [];
    public Dictionary<string, AudioOutputDevice> outputDevices = [];
    public List<IReadMix> iReadMix = [];

    public AudioMixer()
    {
        
    }

    public AudioFrameCollection GetFrames(uint frameCount, uint channelCount, GetSamplesArgs? args = null)
    {
        AudioFrameCollection frames = new AudioFrameCollection((int)frameCount);

        for (int i = 0; i < iReadMix.Count; i++)
        {
            AudioFrameCollection audioFrames = iReadMix[i].ReadMix();
            frames += audioFrames;
        }
        
        foreach (var device in inputDevices)
        {
            AudioFrameCollection deviceFrames = device.Value.ReadMix();
            frames += deviceFrames;
        }

        //if (PlayBleep)
        //{ 
        //    float[] bleepSamples = BleepPlayer.GetSamples(frameCount, channelCount);
        //    mixedSamples = Calculator.Add(mixedSamples, bleepSamples);
        //}

        frames.Clamp(-1, 1);

        return frames;
    }
}