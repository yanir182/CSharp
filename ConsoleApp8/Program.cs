using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.IO;

namespace ConsoleApp8
{
    class Program
    {
        static Process chill = null;
        static string lastProcessOutput = "";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Введите команду (1: запустить, 2: инфо, 3: вывод, 4: стоп, 5: записать в лог):");
                switch (Console.ReadLine())
                {
                    case "1":
                        startpr();
                        break;
                    case "2":
                        Info();
                        break;
                    case "3":
                        lastProcessOutput = routou();
                        break;
                    case "4":
                        stop();
                        break;
                    case "5":
                        lll(lastProcessOutput); 
                        break;
                    default:
                        Console.WriteLine("хзвщ (Неверная команда)");
                        break;
                }
            }
        }

        static void startpr()
        {
            if (chill != null && !chill.HasExited)
            {
                Console.WriteLine("process uje zapushen");
                return;
            }

            Console.WriteLine("vvedi imia programi (naprimer cmd.exe)");
            string p = Console.ReadLine();
            Console.WriteLine("vvedi argument (naprimer /c echo Hello World)");
            string p1 = Console.ReadLine();

            var startinfo = new ProcessStartInfo
            {
                FileName = p,
                Arguments = p1,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true 
            };

            try
            {
                chill = Process.Start(startinfo);
                Console.WriteLine("Process Zapushen");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"хзвщ (Ошибка запуска: {ex.Message})");
            }
        }

        static void Info()
        {
            if (chill == null)
            {
                Console.WriteLine("process ne zapushen");
                return;
            }

            if (!chill.HasExited)
            {
                chill.Refresh();
                Console.WriteLine($"indificator {chill.Id}");
                Console.WriteLine($"HasExited {chill.HasExited}");
                Console.WriteLine($"Start Time {chill.StartTime}");
                Console.WriteLine($"time {chill.TotalProcessorTime}");
                Console.WriteLine($"Pamit {chill.WorkingSet64} bait");
            }
            else
            {
                Console.WriteLine($"Process zaverchen. Kod zavershenia: {chill.ExitCode}");
            }
        }

        static string routou()
        {
            if (chill == null)
            {
                Console.WriteLine("process ne zapushen");
                return "";
            }

            try
            {
                if (!chill.HasExited)
                {
                    Console.WriteLine("Process eshe rabotaet. Ozhidanie zavershenia dlya chtenia vivoda...");
                    chill.WaitForExit();
                }

                string str = chill.StandardOutput.ReadToEnd();
                Console.WriteLine("Vivod processa:");
                Console.WriteLine(str);
                return str;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"хзвщ (Ошибка чтения вывода: {ex.Message})");
                return "";
            }
        }

        static void stop()
        {
            if (chill == null)
            {
                Console.WriteLine("process ne zapushen");
                return;
            }
            try
            {
                if (!chill.HasExited)
                {
                    chill.Kill();
                    chill.WaitForExit();
                    Console.WriteLine("process zaverchon (ubien)");
                }
                else
                {
                    Console.WriteLine("process uje zaverchon");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"хзвщ (Ошибка остановки: {ex.Message})");
            }
        }


        static void lll(string output)
        { 
            try
            {
                using (StreamWriter writer = new StreamWriter("log.txt", true))
                {
                    writer.WriteLine($"[{DateTime.Now}] Vivod processa s ID {chill.Id}:");
                    writer.WriteLine(output);
                    writer.WriteLine("-----------------------------------------------------");
                }
                Console.WriteLine("vivod zapisan v log.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи в файл: {ex.Message}");
            }

        }
    }
}