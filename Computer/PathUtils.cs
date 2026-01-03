namespace CSharpAlgorithms.Computer;
public static class PathUtils
{
    public static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }
}