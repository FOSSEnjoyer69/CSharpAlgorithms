namespace CSharpAlgorithms.Audio;
public interface IAudioProvider
{
    float[] GetSamples(int frameCount, int channelCount);
}
