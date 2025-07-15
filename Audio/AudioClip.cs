using MP3Sharp;

namespace CSharpAlgorithms.Audio;

public class AudioClip
{
    public float[] Samples { get; }
    public int SampleRate { get; }
    public int ChannelCount { get; }
    public string OriginFilePath { get; protected set; } = "";

    public AudioClip(float[] samples, int sampleRate, int channelCount)
    {
        Samples = samples;
        SampleRate = sampleRate;
        ChannelCount = channelCount;
    }

    public override string ToString()
    {
        return $"Sample Count: {Samples.Length}\n" +
               $"Sample Rate: {SampleRate}\n" +
               $"Channel Count: {ChannelCount}";
    }

public static AudioClip FromMP3File(string filePath, int? sampleRate = null)
{
    using var mp3Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    using var mp3 = new MP3Stream(mp3Stream);

    int inputSampleRate = mp3.Frequency;
    int finalRate = sampleRate is not null ? (int)sampleRate : inputSampleRate;
    int channelCount = mp3.ChannelCount;

    using var memoryStream = new MemoryStream();
    byte[] buffer = new byte[4096];
    int bytesRead;

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

    AudioClip clip = new(samples, finalRate, channelCount);
    clip.OriginFilePath = filePath;

    return clip;
}
}