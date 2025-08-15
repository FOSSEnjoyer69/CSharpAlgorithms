#pragma warning disable

using System.Diagnostics;
using System.Net;
using System.Text;

namespace CSharpAlgorithms;
public class Server
{
    private HttpListener m_listener;

    private HtmlFile? m_indexPage = null;

    // Change the type to support async handlers
    private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>> m_customFunctions;
    private Dictionary<string, string> m_filePaths = new Dictionary<string, string>();

    public Server()
    {
        m_customFunctions = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>>();
        m_filePaths = new Dictionary<string, string>();

        m_listener = new HttpListener();
    }

    public async Task Start(int port = -1, bool open=true)
    {
        string localIP = NetworkUtils.GetLocalIPAddress();
        if (port == -1)
            port = NetworkUtils.GetAvailablePort();

        string prefix = $"http://{localIP}:{port}/";
        m_listener.Prefixes.Add(prefix);

        m_listener.Start();
        Console.WriteLine($"CSharpHTTPServer started at {prefix}");

        // Open the server in a browser
        if (open)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = prefix,
                UseShellExecute = true // Ensures the system uses the default web browser
            });
        }

        while (true)
        {
            var context = await m_listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            string path = request.Url.AbsolutePath;
            Console.WriteLine(path);

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
                string text = File.ReadAllText(m_filePaths[path]);
                await Responses.SendTextRespone(response, text);
            }
            else
            {
                await Responses.HandleNotFound(request, response);
            }
        }
    }

    public void AddIndexPage(string filePath)
    {
        HtmlFile file = new HtmlFile(filePath, false);
        m_indexPage = file;
    }

    public void AddCustomPath(string path, Func<HttpListenerRequest, HttpListenerResponse, Task> handler)
    {
        m_customFunctions[path] = handler;
    }

    public void LinkFile(string filePath, string linkPath)
    {
        if (File.Exists(filePath))
        {
            m_filePaths[$"/{linkPath}"] = filePath;
            Console.WriteLine($"Linked {filePath} to {linkPath}");
        }
        else
        {
            Console.WriteLine($"File {filePath} does not exist.");
        }
    }

    private async Task HandleRoot(HttpListenerRequest request, HttpListenerResponse response)
    {
        string responseString = m_indexPage?.Content ?? "Welcome to the server!";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}
