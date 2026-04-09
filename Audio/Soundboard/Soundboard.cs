#define USE_AVALONIA_UI
#define USE_SOUND_FLOW

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



#if USE_AVALONIA_UI
using Avalonia.Controls;
using Avalonia.Interactivity;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;
using SoundFlow.Structs;
#endif

namespace CSharpAlgorithms.Audio;

public class SoundBoard
{
    // public List<AudioDevice> outputAudioDevices { get; private set; } = [];

    // public Dictionary<string, AudioClip> audioClipCache { get; private set; } = [];
    // public Dictionary<string, AudioPlayer> audioPlayerDic { get; private set; } = [];
    public DirectoryInfo RootDirectory { get; private set; }

    public Dictionary<string, SoundPlayer> soundPlayerDic = [];

#if USE_AVALONIA_UI
    public WrapPanel soundBoardPanel { get; private set; }
#endif

#if USE_SOUND_FLOW
    public Dictionary<string, AudioPlaybackDevice> playbackDeviceDic { get; private set; }
    public MiniAudioEngine SoundFlowAudioEngine { get; private set; }
#endif

    public const string CALL_PATH = "[CSharpAlgorithms.Audio.SoundBoard]";

    public SoundBoard
    (
        DirectoryInfo rootDirectory

#if USE_AVALONIA_UI
        , WrapPanel soundBoardPanel
#endif
#if USE_SOUND_FLOW
        , Dictionary<string, AudioPlaybackDevice> playbackDeviceDic, MiniAudioEngine soundFlowAudioEngine
#endif
    )
    {
        this.RootDirectory = rootDirectory;

#if USE_AVALONIA_UI
        this.soundBoardPanel = soundBoardPanel;
#endif
#if USE_SOUND_FLOW
        this.playbackDeviceDic = playbackDeviceDic;
        this.SoundFlowAudioEngine = soundFlowAudioEngine;
#endif

        LoadAudioFromDirectory(rootDirectory);
    }

    public async Task<bool> LoadAudioFromDirectory(DirectoryInfo directoryInfo)
    {
        const string FUNC_CALL_PATH = $"[{CALL_PATH}.LoadAudioFromDirectory(DirectoryInfo directoryInfo)]";
        if (!directoryInfo.Exists)
        {
            Console.WriteLine($"{FUNC_CALL_PATH} - Directory does not exist: {directoryInfo.FullName}");
            return false;
        }

        Debug.WriteSuccess($"{FUNC_CALL_PATH} - Loading audio from directory: {directoryInfo.FullName}");

        (DirectoryInfo[] subDirs, FileInfo[] audioFiles) = GetDirectoryContent(directoryInfo);

#if USE_AVALONIA_UI
        soundBoardPanel.Children.Clear();
#endif
        if (directoryInfo != RootDirectory)
        {
            Button parentDirectoryButton = new Button
            {
                Name = "ParentDirectoryBtn",
                Content = "<--",
                Margin = new Avalonia.Thickness(5),
                Padding = new Avalonia.Thickness(10, 5),
            };

            parentDirectoryButton.Click += async (sender, e) =>
            {
                DirectoryInfo parentDir = directoryInfo.Parent;
                if (parentDir != null)
                    await LoadAudioFromDirectory(parentDir);
            };

            soundBoardPanel.Children.Add(parentDirectoryButton);
        }

        foreach (DirectoryInfo subDirectory in subDirs)
        {
            if (TryCreateSubDirPanel(out StackPanel panel, subDirectory, playButtonClickHandler: async (_, _) => await LoadAudioFromDirectory(subDirectory)))
            {
                soundBoardPanel.Children.Add(panel);
            }
        }

        foreach (FileInfo file in audioFiles)
        {
            string soundName = GetSoundName(file.FullName);

            if (TryCreateSoundPanel(out StackPanel panel, file, playButtonClickHandler: PlaySound))
            {
                soundBoardPanel.Children.Add(panel);
            }
        }

        return false;
    }

#if USE_AVALONIA_UI
    private void PlaySound(object sender, RoutedEventArgs e)
    {
        if (sender is Button playButton)
        {
            string soundName = playButton.Name.ToString();
            PlaySound(soundName);
        }
    }
#endif

    public async Task<bool> PlaySound(string soundId)
    {
        Console.WriteLine($"playing {soundId}");

#if USE_SOUND_FLOW
        PlaySoundOnSoundFlowDevices(soundId);
#endif

        // foreach (AudioDevice device in outputAudioDevices)
        // {
        //     string clipId = GetSoundId(soundId, device);
        //     if (!audioClipCache.TryGetValue(clipId, out AudioClip clip))
        //     {
        //         string filePath = Path.Combine("Audio Clips", soundId);
        //         if (!File.Exists(filePath))
        //         {
        //             Console.WriteLine($"Audio file not found: {filePath}");
        //             continue;
        //         }

        //         clip = await AudioClip.FromMP3File(filePath, device.Info.defaultSampleRate, (short)device.OutputChannelCount);
        //         audioClipCache[clipId] = clip;
        //     }

        //     string playerId = GetPlayerId(clipId, device);
        //     if (!audioPlayerDic.TryGetValue(playerId, out AudioPlayer player))
        //     {
        //         player = new AudioPlayer(clip);
        //         audioPlayerDic[playerId] = player;
        //         device.audioPlayers.Add(player);
        //     }

        //     player.Play();
        // }

        return true;
    }

#if USE_SOUND_FLOW
    public bool PlaySoundOnSoundFlowDevices(string soundId)
    {
        string filePath = GetSoundFilePath(soundId);

        foreach (AudioPlaybackDevice device in playbackDeviceDic.Values)
        {
            string playerId = GetPlayerId(soundId, device);
            if (soundPlayerDic.TryGetValue(soundId, out SoundPlayer player) == false)
            {
                FileStream audioStream = File.OpenRead(filePath);
                StreamDataProvider dataProvider = new StreamDataProvider(SoundFlowAudioEngine, audioStream);

                player = new SoundPlayer(SoundFlowAudioEngine, AudioFormat.DvdHq, dataProvider);
                soundPlayerDic[playerId] = player;

                device.MasterMixer.AddComponent(player);
            }

            player.Play();
            Debug.WriteLine(7);
        }

        return true;
    }
#endif

    public string GetSoundFilePath(string soundId) => Path.Combine(RootDirectory.FullName, soundId);

    public static string GetSoundName(FileInfo fileInfo) => GetSoundName(fileInfo.FullName);
    public static string GetSoundName(string filePath) => Path.GetFileName(filePath);

    public static string GetSoundId(string soundName, int sampleRate) => $"{soundName}:{sampleRate}";
    public static string GetSoundId(string soundName, AudioDevice device) => GetSoundId(soundName, (int)device.Info.defaultSampleRate);

    public static string GetPlayerId(string soundId, string deviceName) => $"{soundId}:{deviceName}";
    public static string GetPlayerId(string soundId, AudioPlaybackDevice device) => $"{soundId}:{device.Info?.Name}";

    public static (DirectoryInfo[] subDirs, FileInfo[] audioFiles) GetDirectoryContent(DirectoryInfo directoryInfo)
    {
        DirectoryInfo[] subDirs = directoryInfo.GetDirectories();
        FileInfo[] audioFiles = [.. directoryInfo.GetFiles().Where(f => f.Extension.Equals(".wav", StringComparison.OrdinalIgnoreCase) || f.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))];

        Array.Sort(subDirs, (a, b) => a.Name.CompareTo(b.Name));
        Array.Sort(audioFiles, (a, b) => a.Name.CompareTo(b.Name));

        return (subDirs, audioFiles);
    }

    public static bool TryCreateSubDirPanel(out StackPanel panel, DirectoryInfo subDirectory, EventHandler<RoutedEventArgs> playButtonClickHandler = null)
    {
        string name = subDirectory.Name;

        panel = new StackPanel
        {
            Name = name,
        };

        Button playButton = new Button
        {
            Name = subDirectory.FullName,
            Content = name,
            Margin = new Avalonia.Thickness(5),
            Padding = new Avalonia.Thickness(10, 5),
        };

        playButton.Click += playButtonClickHandler;

        panel.Children.Add(playButton);

        return true;
    }
    public static bool TryCreateSoundPanel(out StackPanel panel, FileInfo audioFile, EventHandler<RoutedEventArgs> playButtonClickHandler = null)
    {
        string name = GetSoundName(audioFile);

        panel = new StackPanel
        {
            Name = name,
        };

        Button playButton = new Button
        {
            Name = audioFile.FullName,
            Content = name,
            Margin = new Avalonia.Thickness(5),
            Padding = new Avalonia.Thickness(10, 5),
        };

        playButton.Click += playButtonClickHandler;

        panel.Children.Add(playButton);

        return true;
    }
}