namespace CSharpAlgorithms.Audio;

public interface IWriteToMix
{
    public void WriteToMix(AudioFrame[] frames);
}