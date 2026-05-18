using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class SimpleCharServer
{
    const string f = "f.txt";

    static void Main()
    {
        var server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        Console.WriteLine("Сервер запущен. Ожидание клиентов…");
        while (true)
        {
            var client = server.AcceptTcpClient();
            Console.WriteLine("Клиент подключился");

            // Обрабатываем клиента в отдельном потоке, чтобы не блокировать accept-цикл
            ThreadPool.QueueUserWorkItem(_ => HandleClient(client));
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            using (FileStream fileStream = new FileStream(
                       f,
                       FileMode.Append,
                       FileAccess.Write,
                       FileShare.Read))
            {
                int b;
                while ((b = stream.ReadByte()) != -1)   // -1 = поток закрыт
                {
                    fileStream.WriteByte((byte)b);
                    fileStream.Flush();                 // сразу сбрасываем на диск
                    Console.Write((char)b);             // отладочный вывод
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при обработке клиента: " + ex.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("\nКлиент отключился 💀💀");
        }
    }
}
