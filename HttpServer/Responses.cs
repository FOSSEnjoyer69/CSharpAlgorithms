using System.Net;
using System.Text;
using System.Threading.Tasks;

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
    public static async Task SendJavascriptResponse(HttpListenerResponse response, string js)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(js);
        response.ContentLength64 = buffer.Length;
        response.ContentType = "application/javascript";
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

    public static async Task HandleMethodNotAllowed(HttpListenerContext context) => await HandleMethodNotAllowed(context.Response);
    public static async Task HandleMethodNotAllowed(HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
        string responseString = "405 - Method Not Allowed";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}