using System.IO;
using System.Linq;

namespace CSharpAlgorithms.Files;

public static class FileUtils
{
    public static string[] GetFileNames(string directoryPath) => GetFileNames(new DirectoryInfo(directoryPath));
    public static string[] GetFileNames(DirectoryInfo directory)
    {
        string[] names = [.. directory.GetFiles().Select(x => x.Name)];
        return names;
    }

}