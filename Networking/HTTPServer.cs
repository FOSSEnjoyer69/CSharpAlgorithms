using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSharpAlgorithms.Networking;
public class HTTPServer
{
    private HttpListener m_listener;
    public string Prefix { get; private set; } = "";
    private string indexPagePath;


    private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>> m_customFunctions;
    private Dictionary<string, string> m_filePaths = new Dictionary<string, string>();
    private Dictionary<string, string> m_folderPaths = new Dictionary<string, string>();

    public HTTPServer()
    {
        m_listener = new HttpListener();

        m_customFunctions = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>>();
    }

    public async Task Start(int port = -1, bool open=false)
    {
        if (port == -1)
            port = NetworkUtils.GetAvailablePort();

        string localIP = NetworkUtils.GetLocalIPAddress();
        string prefix = $"http://{localIP}:{port}/";
        m_listener.Prefixes.Add(prefix);

        m_listener.Start();
        Console.WriteLine($"HTTP Server started at {prefix}");

        if (open)
            OpenInBrowser();

        while (true)
        {
            var context = await m_listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            if (request.Url is null)
            {
                Console.WriteLine("Request URL is null");
                continue;
            }

            string path = request.Url.AbsolutePath;
            Console.WriteLine($"Request for {path}");

            if (m_customFunctions.ContainsKey(path))
                await m_customFunctions[path](request, response);
            else if (path == "/")
                await HandleRoot(request, response);
            else
            {
                await Responses.HandleNotFound(request, response);
                Debug.WriteErrorLine("No handler for path: " + path);
            }
        }
    }
    public void AddIndexPage(string filePath) => indexPagePath = filePath;
    public void AddCustomPath(string path, Func<HttpListenerRequest, HttpListenerResponse, Task> handler)
    {
        m_customFunctions[path] = handler;
    }

    public void LinkFile(string filePath, string linkPath)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Server.LinkFile]";

        if (!File.Exists(filePath))
        {
            Debug.WriteErrorLine($"{CALL_PATH} File {filePath} does not exist.");
            return;
        }
        
        if (linkPath[0] != '/')
            linkPath = "/" + linkPath;


        linkPath = linkPath.ToLower()
                           .Replace(" ", "-");

        m_filePaths[linkPath] = filePath;
        Console.WriteLine($"{CALL_PATH} Linked {filePath} to {linkPath}");
    }

    public void OpenInBrowser()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Prefix,
            UseShellExecute = true // Ensures the system uses the default web browser
        });
    }

    private async Task HandleRoot(HttpListenerRequest request, HttpListenerResponse response)
    {
        string responseString = "Welcome to the server!";
        if (File.Exists(indexPagePath))
            responseString = await File.ReadAllTextAsync(indexPagePath);

        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}