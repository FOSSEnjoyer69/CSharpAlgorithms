using System.Diagnostics;
using System.Globalization;

namespace CSharpAlgorithms.Media.Video;
public static class VideoClipper
{
    public static async Task ExtractClipByFrameRangeAsync(string inputVideo, string outputVideo, int startFrame, int endFrame, double? fps = null, bool reencode = true)
    {
        if (startFrame < 0)
            throw new ArgumentOutOfRangeException(nameof(startFrame));

        if (endFrame < startFrame)
            throw new ArgumentException("End frame must be >= start frame.");

        fps ??= await FFMPegInterface.GetFPS(inputVideo);

        double startTime = startFrame / fps.Value;

        Directory.CreateDirectory(Path.GetDirectoryName(outputVideo) ?? ".");

        // +1 because endFrame is inclusive
        double duration = ((endFrame - startFrame) + 1) / fps.Value;

        string start = startTime.ToString("0.########", CultureInfo.InvariantCulture);
        string dur = duration.ToString("0.########", CultureInfo.InvariantCulture);

        string args;

        if (reencode)
        {
            // More accurate. Can cut at exact frame/time, but slower.
            args =
                $"-y " +
                $"-ss {start} " +
                $"-i \"{inputVideo}\" " +
                $"-t {dur} " +
                $"-c:v libx264 -preset veryfast -crf 18 " +
                $"-c:a aac -b:a 192k " +
                $"\"{outputVideo}\"";
        }
        else
        {
            // Very fast, but starts on nearest keyframe, not always exact.
            args =
                $"-y " +
                $"-ss {start} " +
                $"-i \"{inputVideo}\" " +
                $"-t {dur} " +
                $"-c copy " +
                $"\"{outputVideo}\"";
        }

        await RunProcessAsync("ffmpeg", args);
    }

    private static async Task RunProcessAsync(string fileName, string arguments)
    {
        using Process process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();

        string stderr = await process.StandardError.ReadToEndAsync();
        string stdout = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception(
                $"FFmpeg failed with exit code {process.ExitCode}\n\nSTDOUT:\n{stdout}\n\nSTDERR:\n{stderr}");
        }
    }
}