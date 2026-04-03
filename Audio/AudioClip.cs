using System;
using System.Buffers;
using System.Buffers.Binary;
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
    
    public static async Task<AudioClip> FromMP3File(string filePath, double sampleRate, short? channelCountOverride = null) =>  await FromMP3File(filePath, (int)sampleRate, channelCountOverride);
    public static async Task<AudioClip> FromMP3File(string filePath, int sampleRate, short? channelCountOverride = null)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Audio.AudioClip.FromMP3File]";

        string fileName = Path.GetFileName(filePath);
        Console.WriteLine($"{CALL_PATH} loading {fileName} from {filePath} at sample rate {sampleRate}");

        using var mp3Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var mp3 = new MP3Stream(mp3Stream);

        int inputRate = mp3.Frequency;
        int targetRate = sampleRate; // <-- choose your intended semantics
        short channelCount = channelCountOverride ?? mp3.ChannelCount;

        using var memoryStream = new MemoryStream();
        byte[] buffer = ArrayPool<byte>.Shared.Rent(64 * 1024);
        try
        {
            int bytesRead;
            while ((bytesRead = mp3.Read(buffer, 0, buffer.Length)) > 0)
                memoryStream.Write(buffer, 0, bytesRead);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        // Avoid ToArray() copy
        if (!memoryStream.TryGetBuffer(out ArraySegment<byte> seg))
            seg = new ArraySegment<byte>(memoryStream.ToArray());

        // Convert PCM16LE -> float
        int byteLen = (int)memoryStream.Length;
        int sampleCount = byteLen / 2; // includes all channels (interleaved)
        float[] samples = new float[sampleCount];

        ReadOnlySpan<byte> pcmBytes = seg.AsSpan(0, byteLen);

        // If you ever run on big-endian (rare), this keeps it correct:
        for (int i = 0, bi = 0; i < sampleCount; i++, bi += 2)
        {
            short s = BinaryPrimitives.ReadInt16LittleEndian(pcmBytes.Slice(bi, 2));
            samples[i] = s * (1f / 32768f);
        }

        // Build clip at the TRUE rate of decoded data first
        AudioFrameCollection frames = new(samples: samples, channelCount: (uint)channelCount);
        AudioClip clip = new(fileName, frames, inputRate, channelCount)
        {
            OriginFilePath = filePath
        };

        // Now resample only if needed, and to the rate you actually want
        if (targetRate != inputRate || channelCountOverride is not null && channelCountOverride.HasValue)
        {
            clip = await FFMPegInterface.Resample(clip, targetRate, channelCountOverride.Value).ConfigureAwait(false);
            clip.OriginFilePath = filePath; // in case resample returns a new instance
        }

        Console.WriteLine($"{CALL_PATH} loaded {clip.Name} from {filePath}");
        return clip;
    }


}