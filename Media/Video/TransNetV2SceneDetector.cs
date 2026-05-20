using System.Diagnostics;
using CommunityToolkit.HighPerformance;
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
    private readonly string _inputName;
    private readonly string _outputName;


    public TransNetV2SceneDetector(string onnxModelPath, bool useCuda = false, int cudaDeviceId = 0)
    {
        if (!File.Exists(onnxModelPath))
            throw new FileNotFoundException("TransNetV2 ONNX model not found.", onnxModelPath);

        SessionOptions options = useCuda
            ? SessionOptions.MakeSessionOptionWithCudaProvider(cudaDeviceId)
            : new SessionOptions();

        options.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;

        session = new InferenceSession(onnxModelPath, options);
        _inputName = session.InputMetadata.Keys.First();
        _outputName = session.OutputMetadata.Keys.First();
    }

    public async Task<Range<int>[]> DetectScenesAsync(string videoPath, float threshold = 0.5f, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(videoPath))
            throw new FileNotFoundException("Video file not found.", videoPath);

        List<byte[]> frames = [];
        VideoReader.OpenRead(videoPath, out VideoReader reader, frameSize: new Vector2<int>(Width, Height), channelCount: Channels);

        foreach (byte[] frame in reader.DecodeFramesStream(videoPath))
        {
            frames.Add(frame);
        }

        if (frames.Count == 0)
            throw new InvalidOperationException("No frames decoded from video.");

        float[] predictions = RunPrediction(frames);

        return PredictionsToScenes(predictions, threshold);
    }

    public async IAsyncEnumerable<Range<int>[]> DetectScenesStream(string videoPath, float threshold = 0.5f, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(videoPath))
            throw new FileNotFoundException("Video file not found.", videoPath);

        VideoReader.OpenRead(videoPath, out VideoReader reader, frameSize: new Vector2<int>(Width, Height), channelCount: Channels);

        int framesRead = 0;
        int currentSceneStartIndex = 0;

        List<byte[]> frames = [];
        foreach (byte[] frame in reader.DecodeFramesStream(videoPath))
        {
            cancellationToken.ThrowIfCancellationRequested();

            framesRead++;
            frames.Add(frame);

            if (frames.Count % WindowFrames != 0)
                continue;

            float[] predictions = RunPrediction(frames);

            for (int i = 0; i < predictions.Length; i++)
            {
                if (predictions[i] < threshold)
                    continue;

                int sceneEndPosition = currentSceneStartIndex + i;

                CSDebug.WriteLine($"{currentSceneStartIndex} - {sceneEndPosition}");

                Range<int> scene = new Range<int>(currentSceneStartIndex, sceneEndPosition);
                yield return new[] { scene };

                currentSceneStartIndex = sceneEndPosition;
                frames.Clear();
            }

            if (framesRead % 1 == 0)
                CSDebug.WriteLine($"\r Frames read: {framesRead}");
        }
    }


    private float[] RunPrediction(IReadOnlyList<byte[]> frames)
    {
        List<float> allPredictions = new();

        int frameCount = frames.Count;

        for (int outputStart = 0; outputStart < frameCount; outputStart += CenterLength)
        {
            byte[][] window = BuildWindow(frames, outputStart);

            float[] windowPredictions = RunWindow(window);

            int remaining = frameCount - outputStart;
            int take = Calculator.Min(CenterLength, remaining);

            allPredictions.AddRange(windowPredictions.Take(take));
        }

        return allPredictions.ToArray();
    }

    private static byte[][] BuildWindow(IReadOnlyList<byte[]> frames, int outputStart)
    {
        byte[][] window = new byte[WindowFrames][];

        // The model window starts 25 frames before the output section.
        int sourceStart = outputStart - CenterStart;

        for (int i = 0; i < WindowFrames; i++)
        {
            int sourceIndex = sourceStart + i;

            if (sourceIndex < 0)
                sourceIndex = 0;

            if (sourceIndex >= frames.Count)
                sourceIndex = frames.Count - 1;

            window[i] = frames[sourceIndex];
        }

        return window;
    }

    private float[] RunWindow(byte[][] frames)
    {
        // Most TransNetV2 ONNX exports use:
        // [batch, frames, height, width, channels]
        var input = new DenseTensor<float>(
            new[] { 1, WindowFrames, Height, Width, Channels });

        for (int t = 0; t < WindowFrames; t++)
        {
            byte[] frame = frames[t];

            int src = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    input[0, t, y, x, 0] = frame[src++];
                    input[0, t, y, x, 1] = frame[src++];
                    input[0, t, y, x, 2] = frame[src++];
                }
            }
        }

        var inputTensor = NamedOnnxValue.CreateFromTensor(_inputName, input);
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results =
            session.Run(new[] { inputTensor });

        Tensor<float> output = results.First(x => x.Name == _outputName).AsTensor<float>();

        float[] raw = output.ToArray();

        // Expected shape is usually [1, 100, 1] or [1, 100].
        // We only keep frames 25..74, same as the original TransNetV2 code.
        float[] center = new float[CenterLength];

        for (int i = 0; i < CenterLength; i++)
        {
            float value = raw[CenterStart + i];

            // Some ONNX exports output logits instead of probabilities.
            // If it looks outside 0..1, apply sigmoid.
            if (value < 0f || value > 1f)
                value = Calculator.Sigmoid(value);

            center[i] = value;
        }

        return center;
    }

    private static Range<int>[] PredictionsToScenes(float[] predictions, float threshold, bool closeFinalScene = true)
    {
        List<Range<int>> scenes = new();

        int start = 0;
        int previous = 0;

        for (int i = 0; i < predictions.Length; i++)
        {
            int current = predictions[i] > threshold ? 1 : 0;

            // Transition ended.
            if (previous == 1 && current == 0)
            {
                start = i;
            }

            // Transition started.
            if (previous == 0 && current == 1 && i != 0)
            {
                scenes.Add(new Range<int>(start, i));
            }

            previous = current;
        }

        // Always close the final real scene at the actual end of the video.
        if (closeFinalScene && predictions.Length > 0)
        {
            int finalFrame = predictions.Length - 1;

            if (scenes.Count == 0 || scenes[^1].Max < finalFrame)
            {
                scenes.Add(new Range<int>(start, finalFrame));
            }
        }

        return scenes.ToArray();
    }

    public void Dispose()
    {
        session?.Dispose();
    }
}