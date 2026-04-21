namespace CSharpAlgorithms.Media;
public static class FFMPegArgsPresets
{
    public static string[] GetScaleVideoArgs(string videoPath, uint width, uint height)
    {
        return
        [
            "-hide_banner",
            "-nostdin",
            "-loglevel", "error",

            "-i", videoPath,

            "-map", "0:v:0",
            "-an",
            "-sn",
            "-dn",

            "-vf", $"scale={width}:{height}:flags=fast_bilinear,format=rgb24",

            "-f", "rawvideo",
            "-vcodec", "rawvideo",
            "pipe:1"
        ];
    }
}