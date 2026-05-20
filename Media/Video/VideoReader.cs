using System.Diagnostics;
using CSharpAlgorithms.Colour;
using CSharpAlgorithms.Computer;
using CSharpAlgorithms.Math;

namespace CSharpAlgorithms.Media.Video;

public sealed class VideoReader
{
    public readonly string VideoPath;
    public readonly Vector2<int> FrameSize;
    public readonly uint ChannelCount;

    public int FrameBytes => FrameSize.X * FrameSize.Y * (int)ChannelCount;

    private VideoReader(string videoPath, Vector2<int>? frameSize = null, uint? channelCount = null)
    {
        frameSize ??= FFMPegInterface.GetVideoResolution(videoPath).GetAwaiter().GetResult();
        channelCount ??= FFMPegInterface.GetVideoChannelCount(videoPath).GetAwaiter().GetResult();

        VideoPath = videoPath;
        FrameSize = frameSize.Value;
        ChannelCount = channelCount.Value;
    }

    public async Task<RGBAColour<byte>[]> DecodeFramesAsync(CancellationToken cancellationToken = default)
    {
        CSDebug.WriteLine($"Decoding video frames from: {VideoPath}");

        ProcessStartInfo psi = FFMPegInterface.CreateFFMPEGStartInfo(VideoPath, FrameSize.X, FrameSize.Y);
        using Process process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start ffmpeg.");

        // Drain stderr while stdout is being read.
        Task<string> errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        List<RGBAColour<byte>> frames = new();
        Stream stdout = process.StandardOutput.BaseStream;

        while (true)
        {
            byte[] frame = new byte[FrameBytes];

            int read = await StreamUtils.ReadFullOrEndAsync(stdout, frame, cancellationToken);

            if (read == 0)
                break;

            if (read != FrameBytes)
                throw new InvalidOperationException(
                    $"Partial frame read from ffmpeg. Got {read} of {FrameBytes} bytes.");

            frames.Add(new RGBAColour<byte>(frame));
        }

        await process.WaitForExitAsync(cancellationToken);
        string error = await errorTask;

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"ffmpeg failed: {error}");

        return [.. frames];
    }

    public IEnumerable<byte[]> DecodeFramesStream(string videoPath)
    {
        CSDebug.WriteLine($"Decoding video frames from: {videoPath}");

        ProcessStartInfo psi = FFMPegInterface.CreateFFMPEGStartInfo(videoPath, FrameSize.X, FrameSize.Y);
        using Process process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start ffmpeg.");

        byte[] buffer = new byte[FrameBytes];
        Stream stdout = process.StandardOutput.BaseStream;

        while (true)
        {
            byte[] frame = new byte[FrameBytes];

            int read = StreamUtils.ReadFullOrEnd(stdout, frame);

            if (read == 0)
                break;

            if (read != FrameBytes)
                throw new InvalidOperationException(
                    $"Partial frame read from ffmpeg. Got {read} of {FrameBytes} bytes.");

            yield return frame;
        }

        process.WaitForExit();
        string error = process.StandardError.ReadToEnd();


        if (process.ExitCode != 0)
            throw new InvalidOperationException($"ffmpeg failed: {error}");

        yield break;
    }

















    //STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS || STATIC FUNCIONS
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static bool OpenRead(string videoPath, out VideoReader reader, Vector2<int>? frameSize = null, uint? channelCount = null)
    {
        reader = null;

        if (string.IsNullOrWhiteSpace(videoPath))
            return false;

        if (!File.Exists(videoPath))
            return false;

        try
        {
            reader = new VideoReader(videoPath, frameSize, channelCount);
            return true;
        }
        catch (Exception ex)
        {
            CSDebug.WriteErrorLine($"Failed to open video: {ex.Message}");
            return false;
        }
    }
}