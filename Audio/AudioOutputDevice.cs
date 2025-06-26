using SoundIOSharp;

namespace CSharpAlgorithms.Audio;

public class AudioOutputDevice
{
    public SoundIODevice OutputDevice { get; private set; }
    public SoundIOOutStream OutputStream { get; private set; }

    Action<IntPtr, double> writeSample;

    double secondsOffset = 0;

    public bool PlayBeep { get; set; } = false;

    private Action<SoundIOOutStream, int, int> callbackLock; //prevents "Process terminated. A callback was made on a garbage collected delegate of type 'VirtualSoundboard!SoundIOSharp.SoundIoOutStream+WriteCallback::Invoke'.."

    public AudioOutputDevice()
    {
        AudioUtils.Initialize();

        OutputDevice = AudioUtils.SoundIO.GetDefaultOutputDevice();
        OutputStream = new SoundIOOutStream(OutputDevice);
        OutputStream.SoftwareLatency = 0.05;

        // Set the format and other properties as needed
        OutputStream.Format = SoundIoFormat.Float32LE;
        OutputStream.Format = SoundIoFormat.Float32LE;
        OutputStream.OnWriteCallback = callbackLock = WriteCallback;

        writeSample = AudioUtils.WriteSampleS32NE;

        OutputStream.Open();
        OutputStream.Start();
    }

    private void WriteCallback(SoundIOOutStream stream, int frameCountMin, int frameCountMax)
    {
        double floatSampleRate = stream.SampleRate;
        double secondsPerFrame = 1.0 / floatSampleRate;

        int framesLeft = frameCountMax;

        SoundIOChannelAreas areas;
        SoundIoError error;

        while (true)
        {
            int frameCount = framesLeft;

            error = stream.BeginWrite(out areas, ref frameCount);
            if (error != SoundIoError.None)
                throw new Exception($"Unrecoverable stream error {error.GetErrorMessage()}");

            if (areas == null || frameCount == 0)
                break;

            SoundIOChannelLayout channelLayout = stream.Layout;

            double pitch = 1000;
            double radiansPerSecond = pitch * 2 * Math.PI;

            for (int frame = 0; frame < frameCount; frame++)
            {
                double sample = 0;

                if (PlayBeep)
                    sample += Math.Clamp(Math.Sin((secondsOffset + frame * secondsPerFrame) * radiansPerSecond) * 0.5, 0, 1);

                for (int channel = 0; channel < channelLayout.ChannelCount; channel++)
                {
                    writeSample(areas[channel].Pointer, sample);
                    areas[channel].AdvancePointer();
                }
            }

            secondsOffset = (secondsOffset + secondsPerFrame * frameCount) % 1;

            error = stream.EndWrite();
            if (error != SoundIoError.None)
                throw new Exception($"EndWrite() failed with error {error.GetErrorMessage()}");

            framesLeft -= frameCount;
            if (framesLeft <= 0)
                break;
        }
    }
}