namespace CSharpAlgorithms.Audio;
public interface IAudioProvider
{
    AudioFrameCollection GetFrames(uint frameCount, uint channelCount, GetSamplesArgs? args = null);
}

public struct GetSamplesArgs
{
    public string? DeviceName;
    public bool IncludeMicrophone;
}
