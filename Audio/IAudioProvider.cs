namespace CSharpAlgorithms.Audio;
public interface IAudioProvider
{
    bool IsFinished { get; }
    float[] GetSamples(int frameCount);
}
