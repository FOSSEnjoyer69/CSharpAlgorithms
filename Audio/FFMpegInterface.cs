#pragma warning disable

using System;
using System.Diagnostics;
using System.IO;

namespace CSharpAlgorithms.Audio;

public static class FFMPegInterface
{
    public static void Resample(string path, int sampleRate)
    {
        string tempPath = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path) + "_temp.mp3");

        string arguments = $"-y -i \"{path}\" -ar {sampleRate} \"{tempPath}\"";

        var processInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using (var process = Process.Start(processInfo))
        {
            string output = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"FFmpeg failed:\n{output}");
            }
        }

        // Replace original with temp file
        string backupPath = path + ".bak";
        File.Replace(tempPath, path, backupPath, ignoreMetadataErrors: true);

        // Optionally, delete backup
        File.Delete(backupPath);

    }
}