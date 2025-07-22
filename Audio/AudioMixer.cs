namespace CSharpAlgorithms.Audio;

public class AudioMixer : IAudioProvider
{
    public List<IAudioProvider> sources = [];

    public AudioOutputDevice OutputDevice { get; set; }

    public AudioMixer(AudioOutputDevice outputDevice)
    {
        OutputDevice = outputDevice;
        OutputDevice.SetAudioProvider(this);
    }

    public float[] GetSamples(int frameCount, int channelCount)
    {
        int sampleCount = frameCount * channelCount;
        float[] mixedSamples = new float[sampleCount];

        for (int sourceIndex = 0; sourceIndex < sources.Count; sourceIndex++)
        {
            float[] sourceSamples = sources[sourceIndex].GetSamples(frameCount, channelCount);

            // Safety: some sources may return fewer samples
            int availableSamples = Math.Min(sampleCount, sourceSamples.Length);

            for (int i = 0; i < availableSamples; i++)
            {
                mixedSamples[i] += sourceSamples[i];
            }
        }

        // Optional: Clamp to avoid clipping
        for (int i = 0; i < mixedSamples.Length; i++)
        {
            mixedSamples[i] = Math.Clamp(mixedSamples[i], -1f, 1f);
        }

        return mixedSamples;
    }

}
