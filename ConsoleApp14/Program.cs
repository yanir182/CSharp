using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace ConsoleApp14
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 5000;
            TcpListener server =
                new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("Сервер запущен...");
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Клиент подключен");
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[2048];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string massage =
                Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Уведомление - " + massage);
            string response = "Сообщение полученно сервером";
            byte[] responseData =
                Encoding.UTF8.GetBytes(response);
            stream.Write(responseData, 0, responseData.Length);
            client.Close();
            server.Stop();
            Console.ReadKey();
        }
    }
}
