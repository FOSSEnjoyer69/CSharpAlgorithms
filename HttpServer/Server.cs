#pragma warning disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAlgorithms;
public class Server
{
    public string Prefix { get; private set; } = "";

    private HttpListener m_listener;

    private string indexPagePath;

    // Change the type to support async handlers
    private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>> m_customFunctions;
    private Dictionary<string, string> m_filePaths = new Dictionary<string, string>();
    private Dictionary<string, string> m_folderPaths = new Dictionary<string, string>();

    public Server()
    {
        m_customFunctions = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>>();
        m_filePaths = new Dictionary<string, string>();

        m_listener = new HttpListener();
    }

    public async Task Start(int port = -1, bool open=true)
    {
        const string CALL_PATH = "[CSharpAlgorithms.Server.Start]";

        string localIP = NetworkUtils.GetLocalIPAddress();
        if (port == -1)
            port = NetworkUtils.GetAvailablePort();

        Prefix = $"http://{localIP}:{port}/";
        m_listener.Prefixes.Add(Prefix);

        m_listener.Start();
        Console.WriteLine($"CSharpHTTPServer started at {Prefix}");

        if (open)
            OpenInBrowser();

        while (true)
        {
            var context = await m_listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            if (request.Url is null)
            {
                Debug.WriteErrorLine($"{CALL_PATH} Request URL is null");
                continue;
            }

            string path = request.Url.AbsolutePath;
            Console.WriteLine($"{CALL_PATH} Request for {path}");

            if (path == "/")
                await HandleRoot(request, response);
            else if (m_customFunctions.ContainsKey(path))
            {
                // Call the custom async function if it exists
                var handler = m_customFunctions[path];
                await handler(request, response);
            }
            else if (m_filePaths.ContainsKey(path))
            {
                string filePath = m_filePaths[path];
                FileInfo fileInfo = new FileInfo(filePath);
                string text = File.ReadAllText(filePath);

                if (fileInfo.Extension.Equals(".js"))
                {
                    await Responses.SendJavascriptResponse(response, text);
                    continue;
                }
                
                await Responses.SendTextRespone(response, text);
            }
            else
            {
                Debug.WriteErrorLine($"{CALL_PATH} No handler for {path}");
                await Responses.HandleNotFound(request, response);
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
