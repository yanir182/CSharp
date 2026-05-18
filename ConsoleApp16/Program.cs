using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static Dictionary<TcpClient, string> cn =
            new Dictionary<TcpClient, string>();
        static Dictionary<TcpClient, DateTime> vrotp =
            new Dictionary<TcpClient,  DateTime>();

        static void Main()
        {
            TcpListener server =
                new TcpListener(IPAddress.Any, 5000);

            server.Start();
            Console.WriteLine("Чат-сервер запущен...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                clients.Add(client);

                Console.WriteLine("Новый клиент подключён.");
                Console.WriteLine($"Клиентов подключено: {clients.Count}");

                Thread clientThread =
                    new Thread(HandleClient);

                clientThread.Start(client);
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            
            byte[] buffer = new byte[1024];

            try
            {
                int g = stream.Read(buffer, 0, buffer.Length);

                string un = Encoding.UTF8.GetString(buffer, 0, g);
                cn[client] = un;

                BroadcastsystemMessage("черт -" + cn[client] + " подключился");
                while (true)     
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);


                    if (string.IsNullOrWhiteSpace(message))
                    {
                        byte[] w = Encoding.UTF8.GetBytes("воу воу воу это пустое сообщение малышь");
                        stream.Write(w, 0, w.Length);
                        continue; 
                    }

                    DateTime vr = DateTime.Now;
                    if (vrotp.ContainsKey(client))
                    {
                        TimeSpan diff = vr - vrotp[client];
                        if (diff.TotalSeconds < 2)
                        {
                            byte[] w = Encoding.UTF8.GetBytes("воу воу воу помедленей малышь");
                            stream.Write(w, 0, w.Length);
                            continue;
                        }
                    }
                    vrotp[client] = vr;
                    if (message.StartsWith("/users"))
                    {
                        Hh(client,message);
                    }
                    BroadcastMessage(message, client);
                }

            }
            catch
            {

            }
            finally
            {
                BroadcastsystemMessage("черт -" + cn[client] + "отключился");
                string names=cn[client];
                cn.Remove(client);
                clients.Remove(client);
                Console.WriteLine($"Клиентов подключено: {clients.Count}");

                client.Close();

            }
        }
        static void Hh(TcpClient client,string message)
        {
            NetworkStream stream = client.GetStream();

            if (message == "/users")
            {   
                foreach (TcpClient userk in cn.Keys)
                {
                    string name = cn[userk];
                    byte[] data = Encoding.UTF8.GetBytes(name);
                    stream.Write(data, 0, data.Length);

                }
            }
        }
        static void BroadcastMessage(string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (TcpClient client in clients)
            {
                if (client != sender)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }

            Console.WriteLine(message);
        }
        static void BroadcastsystemMessage(string massage)
        {
            string fm = "[Симтема-]" + massage;
            byte[] data = Encoding.UTF8.GetBytes(fm);
            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
            Console.WriteLine(fm);
        }
    }
}