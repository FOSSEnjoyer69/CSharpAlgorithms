#pragma warning disable

using System;
using System.IO;

namespace CSharpAlgorithms;
public class HtmlFile
{
    private string m_path;

    public string Content {get; private set;}

    public HtmlFile(string path, bool autoUpdate = false)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"file not found at {path}");

        FileInfo file = new FileInfo(path);

        m_path = path;
        Content = File.ReadAllText(path);

        if (autoUpdate)
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = file.DirectoryName,
                Filter = m_path,

                NotifyFilter = NotifyFilters.LastWrite
            };

            watcher.Changed += (object source, FileSystemEventArgs e) => 
            {
                Update();
            };

            watcher.EnableRaisingEvents = true;
        }
    }

    public void Update()
    {
        Content = File.ReadAllText(m_path);
    }
}