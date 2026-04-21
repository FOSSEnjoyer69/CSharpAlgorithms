using System.Diagnostics;
using System.Globalization;
using CSharpAlgorithms.Math;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CSharpAlgorithms.Media.Video;

public sealed class TransNetV2SceneDetector : IDisposable
{
    private const int Width = 48;
    private const int Height = 27;
    private const int Channels = 3;
    private const int FrameBytes = Width * Height * Channels;

    private const int WindowFrames = 100;
    private const int CenterStart = 25;
    private const int CenterLength = 50;

    private readonly InferenceSession session;
    private readonly string inputName;
    private readonly string outputName;

    public TransNetV2SceneDetector(string onnxModelPath, bool useCuda = false, int cudaDeviceId = 0, string? outputName = null)
    {
        if (!File.Exists(onnxModelPath))
            throw new FileNotFoundException("TransNetV2 ONNX model not found.", onnxModelPath);

        SessionOptions options = useCuda
            ? SessionOptions.MakeSessionOptionWithCudaProvider(cudaDeviceId)
            : new SessionOptions();

        options.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;

        session = new InferenceSession(onnxModelPath, options);

        inputName = session.InputMetadata.Keys.First();
        this.outputName = outputName ?? session.OutputMetadata.Keys.First();
    }

    public async Task<Range<int>[]> DetectScenesAsync(string videoPath, float threshold = 0.5f, CancellationToken cancellationToken = default)
    {
        const string CALL_PATH = "CSharpAlgorithms.Media.Video.DetectScenesAsync";

        if (!File.Exists(videoPath))
            throw new FileNotFoundException("Video not found.", videoPath);

        CSDebug.WriteLine($"[{CALL_PATH}] Starting scene detection for video: {videoPath}");

        double fps = await GetVideoFpsAsync(videoPath, cancellationToken);

        List<byte[]> frames = await DecodeFramesAsync(videoPath, cancellationToken);

        if (frames.Count == 0)
            throw new InvalidOperationException("No frames decoded from video.");

        float[] predictions = RunPrediction(frames);

        return PredictionsToScenes(predictions, fps, threshold);
    }

    private float[] RunPrediction(List<byte[]> frames)
    {
        List<float> predictions = new(frames.Count);

        int frameCount = frames.Count;

        int remainder = frameCount % CenterLength;
        int endPad = CenterStart + CenterLength - (remainder == 0 ? CenterLength : remainder);

        int paddedLength = CenterStart + frameCount + endPad;

        for (int windowStart = 0; windowStart + WindowFrames <= paddedLength; windowStart += CenterLength)
        {
            DenseTensor<float> input = new(new[] { 1, WindowFrames, Height, Width, Channels });
            Span<float> inputSpan = input.Buffer.Span;

            for (int localFrame = 0; localFrame < WindowFrames; localFrame++)
            {
                int paddedIndex = windowStart + localFrame;
                int sourceFrameIndex = Calculator.ClampInclusive(paddedIndex - CenterStart, 0, frameCount - 1);

                byte[] frame = frames[sourceFrameIndex];

                int dstOffset = localFrame * FrameBytes;

                for (int i = 0; i < FrameBytes; i++)
                    inputSpan[dstOffset + i] = frame[i];
            }

            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results =
                session.Run(new[]
                {
                    NamedOnnxValue.CreateFromTensor(inputName, input)
                });

            DisposableNamedOnnxValue output = results.First(x => x.Name == outputName);
            Tensor<float> outputTensor = output.AsTensor<float>();

            float[] windowPredictions = ExtractFramePredictions(outputTensor, WindowFrames);

            for (int i = CenterStart; i < CenterStart + CenterLength; i++)
            {
                if (predictions.Count >= frameCount)
                    break;

                predictions.Add(windowPredictions[i]);
            }

            Console.Write($"\rProcessed {Calculator.Min(predictions.Count, frameCount)}/{frameCount} frames");
        }

        Console.WriteLine();

        return [.. predictions];
    }

    private static float[] ExtractFramePredictions(Tensor<float> tensor, int expectedFrames)
    {
        float[] raw = [.. tensor];

        if (raw.Length < expectedFrames)
            throw new InvalidOperationException(
                $"Model output has {raw.Length} values, expected at least {expectedFrames}.");

        int stride = raw.Length / expectedFrames;

        float[] predictions = new float[expectedFrames];

        for (int frame = 0; frame < expectedFrames; frame++)
        {
            float value = raw[frame * stride];

            // Some exported ONNX models output logits, others already output probabilities.
            // If outside 0..1, treat as logit and sigmoid it.
            if (value < 0f || value > 1f)
                value = Sigmoid(value);

            predictions[frame] = value;
        }

        return predictions;
    }

    private static Range<int>[] PredictionsToScenes(float[] predictions, double fps, float threshold)
    {
        bool[] binary = [.. predictions.Select(x => x > threshold)];

        List<(int Start, int End)> scenes = new();

        bool previous = false;
        int start = 0;

        for (int i = 0; i < binary.Length; i++)
        {
            bool current = binary[i];

            if (previous && !current)
                start = i;

            if (!previous && current && i != 0)
                scenes.Add((start, i));

            previous = current;
        }

        if (!previous)
            scenes.Add((start, binary.Length - 1));

        if (scenes.Count == 0)
            scenes.Add((0, binary.Length - 1));

        return [.. scenes
            .Where(s => s.End > s.Start)
            .Select(s => new Range<int>(s.Start, s.End))];
    }

    private static async Task<List<byte[]>> DecodeFramesAsync(string videoPath, CancellationToken cancellationToken)
    {
        CSDebug.WriteLine($"Decoding video frames from: {videoPath}");
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
        psi.ArgumentList.Add("-i");
        psi.ArgumentList.Add(videoPath);

        psi.ArgumentList.Add("-map");
        psi.ArgumentList.Add("0:v:0");

        psi.ArgumentList.Add("-vf");
        psi.ArgumentList.Add($"scale={Width}:{Height}");

        psi.ArgumentList.Add("-pix_fmt");
        psi.ArgumentList.Add("rgb24");

        psi.ArgumentList.Add("-f");
        psi.ArgumentList.Add("rawvideo");
        psi.ArgumentList.Add("pipe:1");

        using Process process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start ffmpeg.");

        List<byte[]> frames = new();
        byte[] buffer = new byte[FrameBytes];

        while (true)
        {
            int read = 0;

            while (read < FrameBytes)
            {
                int n = await process.StandardOutput.BaseStream.ReadAsync(
                    buffer.AsMemory(read, FrameBytes - read),
                    cancellationToken);

                if (n == 0)
                    break;

                read += n;
            }

            if (read == 0)
                break;

            if (read != FrameBytes)
                throw new InvalidOperationException("Partial frame read from ffmpeg.");

            byte[] frame = new byte[FrameBytes];
            Buffer.BlockCopy(buffer, 0, frame, 0, FrameBytes);
            frames.Add(frame);
        }

        string error = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"ffmpeg failed: {error}");

        return frames;
    }

    private static async Task<double> GetVideoFpsAsync(string videoPath, CancellationToken cancellationToken)
    {
        ProcessStartInfo psi = new()
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
        psi.ArgumentList.Add("stream=avg_frame_rate");
        psi.ArgumentList.Add("-of");
        psi.ArgumentList.Add("default=noprint_wrappers=1:nokey=1");
        psi.ArgumentList.Add(videoPath);

        using Process process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start ffprobe.");

        string output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        string fpsText = output.Trim();

        if (fpsText.Contains('/'))
        {
            string[] parts = fpsText.Split('/');

            if (double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double num) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double den) &&
                den != 0)
            {
                return num / den;
            }
        }

        if (double.TryParse(fpsText, NumberStyles.Float, CultureInfo.InvariantCulture, out double fps))
            return fps;

        throw new InvalidOperationException($"Could not parse FPS: {fpsText}");
    }

    private static float Sigmoid(float x)
    {
        return 1f / (1f + MathF.Exp(-x));
    }

    public void Dispose()
    {
        session?.Dispose();
    }
}