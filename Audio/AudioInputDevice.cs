using System.Runtime.InteropServices;
using SoundIOSharp;

namespace CSharpAlgorithms.Audio;

public class AudioInputDevice
{
    public SoundIODevice InputDevice { get; private set; }
    public SoundIOInStream InputStream { get; private set; }

    Action<IntPtr, double> writeSample;
    double secondsOffset = 0;

    static Dictionary<IntPtr, SoundIORingBuffer> ringBuffers = new Dictionary<IntPtr, SoundIORingBuffer>();

    public AudioInputDevice()
    {
        AudioUtils.Initialize();

        InputDevice = AudioUtils.SoundIO.GetDefaultInputDevice();
        if (InputDevice is null)
            throw new SoundIOException("No input device found");

        InputDevice.SortChannelLayouts();

        int sampleRate = prioritizedSampleRates.FirstOrDefault(sr => InputDevice.SupportsSampleRate(sr));
        if (sampleRate == 0)
            sampleRate = InputDevice.SampleRates[0].Max;
    
        SoundIoFormat fmt = prioritizedFormats.FirstOrDefault(f => InputDevice.SupportsFormat(f));
        if (fmt == SoundIoFormat.Invalid)
            fmt = InputDevice.Formats[0];

        InputStream = new SoundIOInStream(InputDevice);
        //InputStream.Format = fmt;
        InputStream.SampleRate = sampleRate;
        InputStream.OnReadCallback = Callback;
        InputStream.SoftwareLatency = 0.05f;

        writeSample = AudioUtils.WriteSampleS32NE;
    }

    private void Callback(SoundIOInStream stream, int frameCountMin, int frameCountMax)
    {
        SoundIORingBuffer buffer = ringBuffers[stream.UserData];
        SoundIOChannelAreas areas;
        SoundIoError error;

        IntPtr writePtr = buffer.WritePointer;
        int freeBytes = buffer.FreeCount;
        int freeCount = freeBytes / stream.BytesPerFrame;

        if (frameCountMin > freeCount)
            throw new SoundIOException("Ring Buffer overflow");

        int writeFramesCount = Math.Min(frameCountMax, freeCount);
        int framesLeft = writeFramesCount;

        while (true)
        {
            int frameCount = framesLeft;

            error = stream.BeginRead(out areas, ref frameCount);
            if (error != SoundIoError.None)
                throw new Exception($"Unrecoverable stream error {error.GetErrorMessage()}");

            if (frameCount == 0)
                break;

            if (areas is null)
            {
                int count = frameCount * stream.BytesPerFrame;
                for (int i = 0; i < count; i++)
                    Marshal.WriteByte(writePtr + 1, 0);
            }
            else
            {
                int channelCount = stream.Layout.ChannelCount;
                int bytesPerSample = stream.BytesPerSample;
                for (int frame = 0; frame < frameCount; frame++)
                {
                    for (int channel = 0; channel < channelCount; channel++)
                    {
                        unsafe
                        {
                            Buffer.MemoryCopy((void*)areas[channel].Pointer, (void*)writePtr, bytesPerSample, bytesPerSample);
                        }

                        areas[channel].AdvancePointer();
                        writePtr += bytesPerSample;
                    }
                }
            }

            error = stream.EndRead();
            if (error != SoundIoError.None)
                throw new Exception($"EndRead() failed with error {error.GetErrorMessage()}");

            framesLeft -= frameCount;
            if (framesLeft <= 0)
                break;
        }

        int advanceBytes = writeFramesCount * stream.BytesPerFrame;
        buffer.AdvanceWritePointer(advanceBytes);
    }
    
     static SoundIoFormat[] prioritizedFormats =
        {
            SoundIoFormats.Float32NE,
            SoundIoFormats.Float32FE,
            SoundIoFormats.S32NE,
            SoundIoFormats.S32FE,
            SoundIoFormats.S24NE,
            SoundIoFormats.S24FE,
            SoundIoFormats.S16NE,
            SoundIoFormats.S16FE,
            SoundIoFormats.Float64NE,
            SoundIoFormats.Float64FE,
            SoundIoFormats.U32NE,
            SoundIoFormats.U32FE,
            SoundIoFormats.U24NE,
            SoundIoFormats.U24FE,
            SoundIoFormats.U16NE,
            SoundIoFormats.U16FE,
            SoundIoFormat.S8,
            SoundIoFormat.U8,
            SoundIoFormat.Invalid,
        };

        static readonly int[] prioritizedSampleRates =
        {
            48000,
            44100,
            96000,
            24000,
            0,
        };

}