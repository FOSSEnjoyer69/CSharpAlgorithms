using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance.Helpers;
using CSharpAlgorithms;
using CSharpAlgorithms.Audio;
using CSharpAlgorithms.Computer;
using CSharpAlgorithms.Networking;
using HtmlAgilityPack;
public class SoundboardWebuiServer
{
    public SoundBoard soundBoard { get; private set; }
    private HTTPServer server = null!;

    public const string CALL_PATH = "[CSharpAlgorithms.Audio.SoundboardWebuiServer]";

    public SoundboardWebuiServer(SoundBoard soundBoard)
    {
        this.soundBoard = soundBoard;
        this.server = new HTTPServer();
    }

    public bool Start()
    {
        const string FUNC_CALL_PATH = $"[{CALL_PATH}.OpenWebInterface()]";
        HTTPServer server = new HTTPServer();

        server.AddCustomPath("/", HandlePageLoad);
        server.AddCustomPath("/api/playAudio", PlayAudioHandler);

        Thread serverThread = new Thread(async () => await server.Start());
        serverThread.Start();

        return true;
    }

    private async Task PlayAudioHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
        string audioPath = req.QueryString["id"];

        if (string.IsNullOrEmpty(audioPath))
        {
            await Responses.SendTextRespone(res, "Error: No audio path provided.");
            return;
        }

        Console.WriteLine(soundBoard.PlaySound(audioPath));
        await Responses.SendTextRespone(res, $"Playing audio: {audioPath}");
    }

    private async Task HandlePageLoad(HttpListenerRequest req, HttpListenerResponse res)
    {
        string directoryPath = $"Audio Clips/{req.QueryString["folder"] ?? ""}";
        DirectoryInfo directoryInfo = new(directoryPath);
        string responseString = BuildHtml(directoryInfo);

        await Responses.SendTextRespone(res, responseString);
    }

    private string BuildHtml(DirectoryInfo directory)
    {
        string htmlTemplate = File.ReadAllText("CSharpAlgorithms/Audio/Soundboard/Webui/SoundboardWebui.html");
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlTemplate);

        HtmlNode body = htmlDocument.DocumentNode.SelectSingleNode("//body");

        HtmlNode soundboardDiv = htmlDocument.GetElementbyId("soundboard");
        soundboardDiv.SetParent(body);

        var directoryContent = SoundBoard.GetDirectoryContent(directory);

        Console.WriteLine(directory.FullName);
        Console.WriteLine(soundBoard.RootDirectory.FullName);

        if (!DirectoryUtils.IsTheSameDirectory(directory, soundBoard.RootDirectory) && TryGetLocalDirectoryPath(directory.Parent, out string previousDirPath))
        {
            HtmlNode previousDirectoryButton = htmlDocument.CreateElement("button");
            previousDirectoryButton.InnerHtml = "<--";
            previousDirectoryButton.SetAttributeValue("onclick", $"window.location.href='/?folder={previousDirPath}'");
            soundboardDiv.AppendChild(previousDirectoryButton);
        }

        foreach (DirectoryInfo subDir in directoryContent.subDirs)
        {
            if (!TryGetLocalDirectoryPath(subDir, out string subDirPath))
                continue;

            HtmlNode dirButton = htmlDocument.CreateElement("button");
            dirButton.InnerHtml = subDir.Name;
            dirButton.SetAttributeValue("onclick", $"window.location.href='/?folder={subDirPath}'");
            soundboardDiv.AppendChild(dirButton);
        }

        foreach (FileInfo audioFile in directoryContent.audioFiles)
        {
            if (!TryGetLocalDirectoryPath(directory, out string fileDirPath))
                continue;

            if (!string.IsNullOrEmpty(fileDirPath))
                fileDirPath += "/";

            HtmlNode fileButton = htmlDocument.CreateElement("button");
            fileButton.InnerHtml = audioFile.Name;
            fileButton.SetAttributeValue("onclick", $"playAudio('{fileDirPath}{audioFile.Name}')");
            soundboardDiv.AppendChild(fileButton);
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }

    private bool TryGetLocalDirectoryPath(DirectoryInfo baseDirectory, out string localPath)
    {
        string rootPath = soundBoard.RootDirectory.FullName;
        string dirPath = baseDirectory.FullName;

        if (rootPath.EndsWith(dirPath))
        {
            localPath = "";
            return true;
        }

        if (dirPath.StartsWith(rootPath))
        {
            localPath = dirPath.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar);
            return true;
        }

        localPath = null!;
        return false;
    }
}