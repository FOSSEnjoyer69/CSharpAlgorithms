using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace CSharpAlgorithms.Audio;
public sealed class AudioBridge
{
    public readonly string Name;

    public readonly AudioCaptureDevice source;
    public readonly SoundPlayer soundPlayer;
    public readonly AudioPlaybackDevice playbackDeviceDestination;
    public readonly MicrophoneDataProvider microphoneDataProvider;

    public AudioBridge(AudioCaptureDevice source, AudioPlaybackDevice destination, MiniAudioEngine audioEngine)
    {
        this.source = source;
        playbackDeviceDestination = destination;

        Name = GetName(source, destination);

        CSDebug.WriteSuccess($"Created audio bridge from '{source.Info?.Name}' to '{destination.Info?.Name}'");

        microphoneDataProvider = new MicrophoneDataProvider(source);
        soundPlayer = new SoundPlayer(audioEngine, AudioFormat.DvdHq, microphoneDataProvider);
        destination.MasterMixer.AddComponent(soundPlayer);
    }

    public void Play(bool play)
    {
        switch (play)
        {
            case true:
                Play();
                break;
            case false:
                Stop();
                break;
        }
    }
    public void Play()
    {
        source.Start();
        playbackDeviceDestination.Start();
        microphoneDataProvider.StartCapture();

        soundPlayer.Play();
        soundPlayer.Mute = false;
    }

    public void Stop()
    {
        soundPlayer.Mute = true;
    }

    public static string GetName(AudioCaptureDevice source, AudioPlaybackDevice destination) => GetName(source.Info.Value, destination.Info.Value);
    public static string GetName(DeviceInfo source, DeviceInfo destination) => $"{source.Name} -> {destination.Name}";
}