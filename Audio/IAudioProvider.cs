namespace CSharpAlgorithms.Audio;
public interface IAudioProvider
{
    AudioFrameCollection GetFrames(uint frameCount);
}