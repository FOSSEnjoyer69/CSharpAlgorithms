namespace CSharpAlgorithms.Audio;

public static class AudioSettings
{
    /// <summary>
    /// Latency in milliseconds
    /// </summary>
    public static uint Latency { get; private set; } = 20;

    /// <summary>
    /// Sample Rate in Hz
    /// </summary>
    public static uint SampleRate { get; private set; } = 44_100;
    public static uint FramesPerBuffer => (SampleRate * Latency) / 1000;
}