using System;
using System.IO;

namespace CSharpAlgorithms.Computer;
public static class FileFinder
{

    public static bool TryFindFile(out FileInfo fileInfo, DirectoryInfo rootDir, string fileName, string? fileExtension = null)
    {
        fileInfo = null;

        if (!rootDir.Exists)
        {
            Console.WriteLine($"Directory does not exist: {rootDir.FullName}");
            return false;
        }

        FileInfo[] files = rootDir.GetFiles("*", SearchOption.AllDirectories);

        foreach (FileInfo file in files)
        {
            if (file.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase) ||
                (fileExtension != null && file.Extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                fileInfo = file;
                return true;
            }
        }

        Console.WriteLine($"File not found: {fileName} in directory: {rootDir.FullName}");
        return false;
        
    }
}