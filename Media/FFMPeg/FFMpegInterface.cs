#pragma warning disable

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpAlgorithms.Audio;

public static class FFMPegInterface
{
    public static async Task Resample(string inputPath, int sampleRate, int channelCount=2, CancellationToken ct = default)
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

    public static async Task<AudioClip> Resample(AudioClip clip, int sampleRate, int channelCount=2)
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
}