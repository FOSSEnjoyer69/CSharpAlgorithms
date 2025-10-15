using System;
using System.IO;
using System.Threading.Tasks;
using MP3Sharp;

namespace CSharpAlgorithms.Audio;

public class AudioClip
{
    public string Name { get; }
    public AudioFrameCollection Frames { get; }
    public int Length => Frames.Length;
    public int SampleRate { get; }
    public int FrameRate { get; }
    public int ChannelCount { get; }
    public int TimeLength { get; }
    public string OriginFilePath { get; protected set; } = "";

    public AudioClip(string name, AudioFrameCollection frames, int sampleRate, int channelCount)
    {
        Name = name;
        Frames = frames;
        SampleRate = sampleRate;
        FrameRate = SampleRate / channelCount;
        ChannelCount = channelCount;
        TimeLength = Length / SampleRate;
    }

    public override string ToString()
    {
        return $"Name: {Name}\n" +
               $"Frame Count: {Frames.Length}\n" +
               $"Sample Rate: {SampleRate}\n" +
               $"Frame Rate: {FrameRate}\n" +
               $"Channel Count: {ChannelCount}\n" +
               $"Time Length: {TimeLength}\n" +
               $"File Path: {OriginFilePath}\n"
               ;
    }

    public static async Task<AudioClip> FromMP3File(string filePath, int? sampleRate = null)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioClip.FromMP3File]";

        string fileName = Path.GetFileName(filePath);

        using var mp3Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var mp3 = new MP3Stream(mp3Stream);

        int inputSampleRate = mp3.Frequency;
        int finalRate = sampleRate ?? inputSampleRate;
        short channelCount = mp3.ChannelCount;

        using var memoryStream = new MemoryStream();
        byte[] buffer = new byte[4096];
        int bytesRead;

        // Important: use synchronous Read, because MP3Sharp decodes here
        while ((bytesRead = mp3.Read(buffer, 0, buffer.Length)) > 0)
        {
            memoryStream.Write(buffer, 0, bytesRead);
        }

        byte[] pcmData = memoryStream.ToArray();
        int sampleCount = pcmData.Length / 2;
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(pcmData, i * 2);
            samples[i] = sample / 32768f;
        }

        AudioFrameCollection frames = new(samples: samples, channelCount: (uint)channelCount);
        AudioClip clip = new(fileName, frames, finalRate, channelCount)
        {
            OriginFilePath = filePath
        };

        if (finalRate != AudioSettings.SampleRate)
            clip = await FFMPegInterface.Resample(clip, (int)AudioSettings.SampleRate);

        Console.WriteLine($"{CALL_PATH} loaded {clip}");

        return clip;
    }


}