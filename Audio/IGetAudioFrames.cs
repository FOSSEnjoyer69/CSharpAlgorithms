namespace CSharpAlgorithms.Audio;

public interface IGetAudioFrames
{
    public AudioFrame[] GetFrames(int frameCount);
}