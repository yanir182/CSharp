using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static int refreshInterval = 2000;
    private static bool isRunning = true;
    private static Process selectedProcess = null;
    private static List<Process> processList = new List<Process>();

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Менеджер процессов ===");

        await RunMainMenu();
    }

    static async Task RunMainMenu()
    {
        while (isRunning)
        {
            Console.Clear();
            Console.WriteLine("=== Менеджер процессов ===");
            Console.WriteLine($"Текущий интервал обновления: {refreshInterval} мс");
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Показать список процессов");
            Console.WriteLine("2. Настроить интервал обновления");
            Console.WriteLine("3. Запустить приложение");
            Console.WriteLine("4. Выйти из программы");

            if (selectedProcess != null)
            {
                Console.WriteLine($"\nВыбран процесс: {selectedProcess.ProcessName} (ID: {selectedProcess.Id})");
            }

            Console.Write("\nВаш выбор: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowProcessList();
                    break;
                case "2":
                    SetRefreshInterval();
                    break;
                case "3":
                    await LaunchApplication();
                    break;
                case "4":
                    isRunning = false;
                    Console.WriteLine("Завершение работы...");
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    break;
            }
        }
    }


    static async Task ShowProcessList()
    {
        bool inProcessMenu = true;

        while (inProcessMenu)
        {
            Console.Clear();
            Console.WriteLine("=== Список процессов ===");
            Console.WriteLine($"Обновление каждые {refreshInterval} мс");
            Console.WriteLine("\nНажмите ESC для возврата в главное меню");
            Console.WriteLine("Введите номер процесса для выбора\n");

            try
            {
                processList = Process.GetProcesses()
                    .OrderBy(p => p.ProcessName)
                    .ToList();

                Console.WriteLine($"{"№",-4} {"Имя процесса",-30} {"ID",-10} {"Память (МБ)",-12}");
                Console.WriteLine(new string('-', 60));

                for (int i = 0; i < processList.Count; i++)
                {
                    var process = processList[i];
                    try
                    {
                        long memoryMB = process.WorkingSet64 / (1024 * 1024);
                        Console.WriteLine($"{i + 1,-4} {process.ProcessName,-30} {process.Id,-10} {memoryMB,-12}");
                    }
                    catch
                    {
                        Console.WriteLine($"{i + 1,-4} {process.ProcessName,-30} {process.Id,-10} {"Н/Д",-12}");
                    }
                }

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        inProcessMenu = false;
                        continue;
                    }

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.Write("\nВведите номер процесса для выбора: ");
                        if (int.TryParse(Console.ReadLine(), out int processNumber) &&
                            processNumber >= 1 && processNumber <= processList.Count)
                        {
                            await SelectProcess(processList[processNumber - 1]);
                        }
                    }
                }

                await Task.Delay(refreshInterval);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении процессов: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                inProcessMenu = false;
            }
        }
    }


    static async Task SelectProcess(Process process)
    {
        bool inDetailMenu = true;

        while (inDetailMenu && !process.HasExited)
        {
            Console.Clear();
            Console.WriteLine("=== Детальная информация о процессе ===");
            Console.WriteLine($"Процесс: {process.ProcessName}");

            try
            {
                process.Refresh();

                Console.WriteLine($"{"ID процесса:",-40} {process.Id}");
                Console.WriteLine($"{"Имя процесса:",-40} {process.ProcessName}");

                try
                {
                    Console.WriteLine($"{"Время старта:",-40} {process.StartTime:yyyy-MM-dd HH:mm:ss}");
                }
                catch
                {
                    Console.WriteLine($"{"Время старта:",-40} Нет доступа");
                }

                try
                {
                    Console.WriteLine($"{"Процессорное время:",-40} {process.TotalProcessorTime:hh\\:mm\\:ss\\.fff}");
                }
                catch
                {
                    Console.WriteLine($"{"Процессорное время:",-40} Нет доступа");
                }

                try
                {
                    Console.WriteLine($"{"Количество потоков:",-40} {process.Threads.Count}");
                }
                catch
                {
                    Console.WriteLine($"{"Количество потоков:",-40} Нет доступа");
                }

                try
                {
                    int instanceCount = Process.GetProcessesByName(process.ProcessName).Length;
                    Console.WriteLine($"{"Количество копий:",-40} {instanceCount}");
                }
                catch
                {
                    Console.WriteLine($"{"Количество копий:",-40} Нет доступа");
                }

                try
                {
                    long memoryMB = process.WorkingSet64 / (1024 * 1024);
                    Console.WriteLine($"{"Используемая память:",-40} {memoryMB} МБ");
                }
                catch
                {
                    Console.WriteLine($"{"Используемая память:",-40} Нет доступа");
                }

                Console.WriteLine($"{"Приоритет:",-40} {process.BasePriority}");

                try
                {
                    Console.WriteLine($"{"Исполняемый файл:",-40} {process.MainModule?.FileName ?? "Нет доступа"}");
                }
                catch
                {
                    Console.WriteLine($"{"Исполняемый файл:",-40} Нет доступа");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении информации: {ex.Message}");
            }

            Console.WriteLine("\nДействия:");
            Console.WriteLine("1. Завершить процесс (Задание 3)");
            Console.WriteLine("2. Вернуться к списку процессов");
            Console.WriteLine("ESC. Вернуться в главное меню");

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:

                        KillProcess(process);
                        inDetailMenu = false;
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        inDetailMenu = false;
                        break;
                    case ConsoleKey.Escape:
                        inDetailMenu = false;
                        selectedProcess = null;
                        return;
                }
            }

            await Task.Delay(2000);
        }

        if (process.HasExited)
        {
            Console.WriteLine("\nПроцесс завершен. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        selectedProcess = null;
    }


    static void KillProcess(Process process)
    {
        Console.Write($"\nВы уверены, что хотите завершить процесс '{process.ProcessName}' (ID: {process.Id})? (да/нет): ");
        string confirmation = Console.ReadLine()?.ToLower();

        if (confirmation == "да" || confirmation == "y" || confirmation == "yes")
        {
            try
            {
                process.Kill();
                Console.WriteLine($"Процесс '{process.ProcessName}' успешно завершен.");
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при завершении процесса: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }

    static void SetRefreshInterval()
    {
        Console.Clear();
        Console.WriteLine("=== Настройка интервала обновления ===");
        Console.WriteLine($"Текущий интервал: {refreshInterval} мс");
        Console.Write("\nВведите новый интервал в миллисекундах (мин. 500, макс. 30000): ");

        if (int.TryParse(Console.ReadLine(), out int newInterval))
        {
            if (newInterval >= 500 && newInterval <= 30000)
            {
                refreshInterval = newInterval;
                Console.WriteLine($"Интервал обновления установлен: {refreshInterval} мс");
            }
            else
            {
                Console.WriteLine("Интервал должен быть от 500 до 30000 мс.");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат числа.");
        }

        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    static async Task LaunchApplication()
    {
        bool inLaunchMenu = true;

        while (inLaunchMenu)
        {
            Console.Clear();
            Console.WriteLine("=== Запуск приложений ===");
            Console.WriteLine("\nВыберите приложение для запуска:");
            Console.WriteLine("1. Блокнот (notepad.exe)");
            Console.WriteLine("2. Калькулятор (calc.exe)");
            Console.WriteLine("3. Paint (mspaint.exe)");
            Console.WriteLine("4. Запустить свой исполняемый файл");
            Console.WriteLine("5. Вернуться в главное меню");

            Console.Write("\nВаш выбор: ");
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Process.Start("notepad.exe");
                        Console.WriteLine("Блокнот запущен.");
                        break;
                    case "2":
                        Process.Start("calc.exe");
                        Console.WriteLine("Калькулятор запущен.");
                        break;
                    case "3":
                        Process.Start("mspaint.exe");
                        Console.WriteLine("Paint запущен.");
                        break;
                    case "4":
                        LaunchCustomApplication();
                        break;
                    case "5":
                        inLaunchMenu = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске приложения: {ex.Message}");
            }

            if (choice != "5")
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }

    static void LaunchCustomApplication()
    {
        Console.Write("\nВведите путь к исполняемому файлу: ");
        string path = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    Process.Start(path);
                    Console.WriteLine($"Приложение запущено: {path}");
                }
                else
                {

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    };

                    Process.Start(startInfo);
                    Console.WriteLine($"Команда запущена: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске: {ex.Message}");
                Console.WriteLine("Попробуйте указать полный путь к файлу.");
            }
        }
        else
        {
            Console.WriteLine("Путь не может быть пустым.");
        }
    }
}