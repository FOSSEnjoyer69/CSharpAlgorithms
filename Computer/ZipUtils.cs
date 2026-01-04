using System.IO;
using System.IO.Compression;

namespace CSharpAlgorithms.Computer;
public static class ZipUtils
{
    public static void UnzipAndCopyContent(string zipFilePath, string destinatiionDirectoryPath)
    {
        using ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath);
        UnzipAndCopyContent(zipArchive, destinatiionDirectoryPath);   
    }

    public static void UnzipAndCopyContent(ZipArchive zipArchive, string destinatiionDirectoryPath)
    {
        foreach (ZipArchiveEntry entry in zipArchive.Entries)
        {
            entry.ExtractToFile(Path.Combine(destinatiionDirectoryPath, entry.Name), true);
        }
    }
}
