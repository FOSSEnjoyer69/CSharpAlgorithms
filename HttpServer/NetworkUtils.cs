using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CSharpAlgorithms;

public static class NetworkUtils
{
    public static string GetLocalIPAddress()
    {
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
                continue;

            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                {
                    return ip.Address.ToString();
                }
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }


    public static int GetAvailablePort(int startPort = 1024, int endPort = 65535)
    {
        for (int port = startPort; port <= endPort; port++)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return port; // Found available port
            }
            catch (SocketException)
            {
                // Port is in use, try next
            }
        }

        throw new Exception("No available ports found in the specified range.");
    }
}