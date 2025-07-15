using System.Collections.Concurrent;

namespace CSharpAlgorithms.Audio;

public class AudioMixer : IAudioBuffer
{
    public BlockingCollection<float[]> AudioBuffer { get; set; } = [];

    public AudioOutputDevice OutputDevice { get; protected set; }

    public List<AudioPlayer> audioPlayers;

    public bool IsRunning { get; private set; }

    private Thread mixThread;
    private readonly object lockObj = new();

    private readonly int framesPerBuffer = 512;

    private const int CHANNEL_COUNT = 2;


    public AudioMixer()
    {
        OutputDevice = new AudioOutputDevice(AudioBuffer);
        audioPlayers = new List<AudioPlayer>();

        framesPerBuffer = (int)(OutputDevice.Info.defaultSampleRate * OutputDevice.Info.defaultLowOutputLatency);
    }

    public void Start()
    {
        if (IsRunning) return;
        IsRunning = true;

        mixThread = new Thread(MixLoop)
        {
            IsBackground = true
        };
        mixThread.Start();
    }

    private void MixLoop()
    {
        while (IsRunning)
        {
            float[] mixBuffer = new float[framesPerBuffer * CHANNEL_COUNT];
            lock (lockObj)
            {
                foreach (AudioPlayer player in audioPlayers)
                {
                    if (!player.IsFinished)
                    {
                        float[] samples = player.GetSamples(framesPerBuffer);
                        for (int i = 0; i < Math.Min(mixBuffer.Length, samples.Length); i++)
                        {
                            mixBuffer[i] += samples[i];
                        }
                    }

                }
            }

            for (int i = 0; i < mixBuffer.Length; i++)
                mixBuffer[i] = Math.Clamp(mixBuffer[i], -1f, 1f);

            AudioBuffer.Add(mixBuffer);
        }
    }

}
