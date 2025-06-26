using SoundIOSharp;

namespace CSharpAlgorithms.Audio;

public class VirtualSoundboard
{
    public AudioInputDevice InputDevice;
    public AudioOutputDevice OutputDevice;

    public VirtualSoundboard()
    {
        //InputDevice = new AudioInputDevice(); //Causes a crash on startup, not sure why
        OutputDevice = new AudioOutputDevice();
    }
    
}