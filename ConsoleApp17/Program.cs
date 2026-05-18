using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        TcpClient client = new TcpClient("192.168.0.3", 5000);
        NetworkStream stream = client.GetStream();

        new Thread(() =>
        {
            byte[] buffer = new byte[2048];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;
                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
        }).Start();
    }
}
