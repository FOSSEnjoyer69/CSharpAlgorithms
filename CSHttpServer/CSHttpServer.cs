using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

    public async Task Start(bool open=true)
    {
        string localIP = GetLocalIPAddress();
        int port = GetAvailablePort();

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
                await HandleNotFound(request, response);
            }
        }
    }

    public void AddIndexPage(string filePath)
    {
        HtmlFile file = new HtmlFile(filePath, false);
        m_indexPage = file;
    }

    // Update to accept async handler
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

    private async Task HandleUpload(HttpListenerRequest request, HttpListenerResponse response)
    {
        // Check if the request method is POST
        if (request.HttpMethod == "POST")
        {
            // Read the form data from the request
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string formData = await reader.ReadToEndAsync();
                Console.WriteLine("Received Form Data: " + formData);

                // Parse the form data (simple parsing for application/x-www-form-urlencoded)
                string[] formDataParts = formData.Split('&');
                foreach (var part in formDataParts)
                {
                    string[] keyValue = part.Split('=');
                    string key = WebUtility.UrlDecode(keyValue[0]);
                    string value = WebUtility.UrlDecode(keyValue[1]);
                    Console.WriteLine($"{key}: {value}");
                }

                // Send a response back to the client
                byte[] buffer = Encoding.UTF8.GetBytes("Form data received successfully");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
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

    private async Task HandleNotFound(HttpListenerRequest request, HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.NotFound;
        string responseString = "404 - Not Found";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }


    static string GetLocalIPAddress()
    {
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private static int GetAvailablePort()
    {
        // Bind to an available port by using TcpListener with port 0
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
