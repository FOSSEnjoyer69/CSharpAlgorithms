#pragma warning disable

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CSharpAlgorithms.Audio;
using CSharpAlgorithms.Math;
using CSharpAlgorithms.Media.Video;

namespace CSharpAlgorithms.Media;

public static class FFMPegInterface
{
    public static async Task Resample(string inputPath, int sampleRate, int channelCount = 2, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            throw new ArgumentException("Input path is required.", nameof(inputPath));

        if (sampleRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(sampleRate), "Sample rate must be > 0.");

        inputPath = Path.GetFullPath(inputPath);

        if (!File.Exists(inputPath))
            throw new FileNotFoundException("Input file not found.", inputPath);

        string dir = Path.GetDirectoryName(inputPath) ?? Environment.CurrentDirectory;
        string tempPath = Path.Combine(dir,
            $"{Path.GetFileNameWithoutExtension(inputPath)}_{Guid.NewGuid():N}.tmp.mp3");

        try
        {
            // If you want to preserve quality more consistently, specify the encoder explicitly.
            // -vn avoids copying any video stream if present.
            string args = $"-y -hide_banner -vn -i \"{inputPath}\" -ac {channelCount} -ar {sampleRate} \"{tempPath}\"";

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = false, // avoid deadlock by not piping stdout
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = new Process { StartInfo = psi };

            if (!process.Start())
                throw new InvalidOperationException("Failed to start ffmpeg process.");

            string stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync(ct);

            if (process.ExitCode != 0)
                throw new Exception($"FFmpeg failed (exit {process.ExitCode}):\n{stderr}");

            // Safer replace with rollback
            string backupPath = inputPath + ".bak";

            // Move original out of the way first
            if (File.Exists(backupPath))
                File.Delete(backupPath);

            File.Move(inputPath, backupPath, overwrite: true);

            try
            {
                File.Move(tempPath, inputPath, overwrite: true);
                File.Delete(backupPath);
            }
            catch
            {
                // Restore original if replacing fails
                if (File.Exists(inputPath))
                    File.Delete(inputPath);

                File.Move(backupPath, inputPath, overwrite: true);
                throw;
            }
        }
        finally
        {
            // Best-effort cleanup
            if (File.Exists(tempPath))
            {
                try { File.Delete(tempPath); } catch { /* ignore */ }
            }
        }
    }

    public static async Task<AudioClip> Resample(AudioClip clip, int sampleRate, int channelCount = 2)
    {
        FileInfo originalFile = new FileInfo(clip.OriginFilePath);
        string tempPath = Path.Combine(Path.GetDirectoryName(originalFile.FullName)!, Path.GetFileNameWithoutExtension(originalFile.FullName) + "_temp.mp3");

        if (File.Exists(tempPath))
            File.Delete(tempPath);

        File.Copy(clip.OriginFilePath, tempPath);

        await Resample(tempPath, sampleRate, channelCount);
        AudioClip resampledClip = await AudioClip.FromMP3File(tempPath, sampleRate);
        File.Delete(tempPath);
        return resampledClip;
    }

    public static async Task<double?> GetFPS(string videoFilePath, CancellationToken ct = default)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "ffprobe",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        psi.ArgumentList.Add("-v");
        psi.ArgumentList.Add("error");

        psi.ArgumentList.Add("-select_streams");
        psi.ArgumentList.Add("v:0");

        psi.ArgumentList.Add("-show_entries");
        psi.ArgumentList.Add("stream=avg_frame_rate");

        psi.ArgumentList.Add("-of");
        psi.ArgumentList.Add("csv=p=0");

        psi.ArgumentList.Add(videoFilePath);

        using Process process = new Process { StartInfo = psi };

        if (!process.Start())
            throw new InvalidOperationException("Failed to start ffmpeg process.");

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(ct);

        string fpsText = output.Trim();

        if (VideoUtils.TryParseFps(fpsText, out double fps))
            return fps;
        else
        {
            CSDebug.WriteErrorLine($"Failed to parse FPS from ffmpeg output: '{fpsText}'. Error: {error}");
            return null;
        }
    }


    public static ProcessStartInfo CreateFFMPEGStartInfo(string videoPath, int Width, int Height)
    {
        ProcessStartInfo psi = new()
        {
            FileName = "ffmpeg",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        psi.ArgumentList.Add("-hide_banner");
        psi.ArgumentList.Add("-loglevel");
        psi.ArgumentList.Add("error");
        psi.ArgumentList.Add("-nostdin");

        // Auto thread count.
        psi.ArgumentList.Add("-threads");
        psi.ArgumentList.Add("0");

        psi.ArgumentList.Add("-i");
        psi.ArgumentList.Add(videoPath);

        // Only decode first video stream.
        psi.ArgumentList.Add("-map");
        psi.ArgumentList.Add("0:v:0");

        // Ignore audio/subtitles/data.
        psi.ArgumentList.Add("-an");
        psi.ArgumentList.Add("-sn");
        psi.ArgumentList.Add("-dn");

        // Scale + RGB conversion in one filter chain.
        // fast_bilinear is usually fine for ML preprocessing.
        psi.ArgumentList.Add("-vf");
        psi.ArgumentList.Add($"scale={Width}:{Height}:flags=fast_bilinear,format=rgb24");

        psi.ArgumentList.Add("-f");
        psi.ArgumentList.Add("rawvideo");
        psi.ArgumentList.Add("pipe:1");

        return psi;
    }

    public static async Task<Vector2<int>> GetVideoResolution(string videoPath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "ffprobe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        psi.ArgumentList.Add("-v");
        psi.ArgumentList.Add("error");

        psi.ArgumentList.Add("-select_streams");
        psi.ArgumentList.Add("v:0");

        psi.ArgumentList.Add("-show_entries");
        psi.ArgumentList.Add("stream=width,height");

        psi.ArgumentList.Add("-of");
        psi.ArgumentList.Add("csv=p=0:s=x");

        psi.ArgumentList.Add(videoPath);

        using var process = new Process { StartInfo = psi };

        if (!process.Start())
            throw new InvalidOperationException("Failed to start ffprobe process.");

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string txt = output.Trim();
        // Expect format: WIDTHxHEIGHT or "WIDTH,HEIGHT" depending; using x separator above
        string[] parts = txt.Split('x', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 2 && int.TryParse(parts[0], out int w) && int.TryParse(parts[1], out int h))
            return new Vector2<int>(w, h);

        CSDebug.WriteErrorLine($"Failed to parse resolution from ffprobe output: '{txt}'. Error: {error}");
        throw new Exception("Unable to determine video resolution.");
    }

    public static async Task<uint> GetVideoChannelCount(string videoPath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "ffprobe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        psi.ArgumentList.Add("-v");
        psi.ArgumentList.Add("error");

        psi.ArgumentList.Add("-select_streams");
        psi.ArgumentList.Add("v:0");

        psi.ArgumentList.Add("-show_entries");
        psi.ArgumentList.Add("stream=nb_read_frames,pix_fmt");

        psi.ArgumentList.Add("-of");
        psi.ArgumentList.Add("csv=p=0");

        psi.ArgumentList.Add(videoPath);

        using var process = new Process { StartInfo = psi };

        if (!process.Start())
            throw new InvalidOperationException("Failed to start ffprobe process.");

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string pixelFormat = output.Trim();

        // Map common pixel formats to channel count
        uint channels = pixelFormat switch
        {
            "gray" or "gray8" => 1,
            "rgb24" or "bgr24" => 3,
            "rgba" or "bgra" => 4,
            "yuv420p" => 3,
            "yuv422p" => 3,
            "yuv444p" => 3,
            _ => 3 // default to RGB
        };

        return channels;
    }

    public static async IAsyncEnumerable<byte[]> DecodeFrames48x27RgbAsync(string videoPath, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const int FrameWidth = 48;
        const int FrameHeight = 27;
        const int ChannelCount = 3;
        const int FrameBytes = FrameWidth * FrameHeight * ChannelCount;

        ProcessStartInfo psi = new()
        {
            FileName = "ffmpeg",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string[] args =
        {
            "-hide_banner",
            "-loglevel", "error",
            "-i", videoPath,
            "-vf", $"scale=48:27",
            "-pix_fmt", "rgb24",
            "-f", "rawvideo",
            "pipe:1"
        };

        foreach (string arg in args)
            psi.ArgumentList.Add(arg);

        using Process process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start ffmpeg.");

        Task<string> stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        Stream output = process.StandardOutput.BaseStream;
        byte[] readBuffer = new byte[FrameBytes];

        while (true)
        {
            int read = 0;

            while (read < FrameBytes)
            {
                int n = await output.ReadAsync(
                    readBuffer.AsMemory(read, FrameBytes - read),
                    cancellationToken);

                if (n == 0)
                    break;

                read += n;
            }

            if (read == 0)
                break;

            if (read != FrameBytes)
                throw new InvalidOperationException("ffmpeg returned a partial frame.");

            byte[] frame = new byte[FrameBytes];
            Buffer.BlockCopy(readBuffer, 0, frame, 0, FrameBytes);

            yield return frame;
        }

        await process.WaitForExitAsync(cancellationToken);

        string stderr = await stderrTask;

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"ffmpeg failed: {stderr}");
    }
}