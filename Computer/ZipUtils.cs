using System.IO;
using System.IO.Compression;

namespace CSharpAlgorithms.Computer
{
    public static class ZipUtils
    {
        public static bool UnzipAndCopyFiles(string zipFilePath, string destinationDirectory)
        {
            FileStream fileStream = File.OpenRead(zipFilePath);
            ZipArchive archive = new(fileStream);

            archive.ExtractToDirectory(destinationDirectory, overwriteFiles: true);


            return true;
        }
    }
}