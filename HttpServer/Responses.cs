using System.Net;
using System.Text;

namespace CSharpAlgorithms;

public static class Responses
{
    public static async Task SendNotPostRespone(HttpListenerResponse response) => await SendTextRespone(response, "Not Post");
    public static async Task SendNoEntityBodyRespone(HttpListenerResponse response) => await SendTextRespone(response, "No EntityBody");

    public static async Task SendJSONResponse(HttpListenerResponse response, string json)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        response.ContentLength64 = buffer.Length;
        response.ContentType = "application/json";
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
    public static async Task SendTextRespone(HttpListenerResponse response, string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    public static async Task HandleNotFound(HttpListenerRequest request, HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.NotFound;
        string responseString = "404 - Not Found";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}