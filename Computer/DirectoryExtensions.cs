using System;
using System.IO;
using System.IO.Compression;

namespace CSharpAlgorithms.Computer;
public static class DirectoryUtils
{
    public static void DeleteUsingTemplate(ZipArchive template, string targetDirectoryPath)
    {
        foreach (ZipArchiveEntry entry in template.Entries)
        {
            string targetPath = Path.Combine(targetDirectoryPath, entry.FullName);
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            else if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }
            else
            {
                Console.WriteLine($"{targetPath} does not exist.");
            }
        }   
    }

    public static bool ContainsTemplate(ZipArchive template, string targetDirectoryPath)
    {
        foreach (ZipArchiveEntry entry in template.Entries)
        {
            string targetPath = Path.Combine(targetDirectoryPath, entry.FullName);
            if (!File.Exists(targetPath) && !Directory.Exists(targetPath))
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsTheSameDirectory(DirectoryInfo dir1, DirectoryInfo dir2)
    {
        return string.Equals(dir1.FullName.TrimEnd(Path.DirectorySeparatorChar), dir2.FullName.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
    }
}