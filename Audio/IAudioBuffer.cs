using System.Collections.Concurrent;
namespace CSharpAlgorithms.Audio;
public interface IAudioBuffer
{
    public BlockingCollection<float[]> AudioBuffer { get; set; }
}