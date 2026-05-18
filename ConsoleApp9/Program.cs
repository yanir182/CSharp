using System;
using System.Runtime.InteropServices;

namespace GuessNumberWinAPI
{
    class Program
    {
        // Импорт функции MessageBox из user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        static void Main(string[] args)
        {
            Console.WriteLine("Программа использует Windows API для взаимодействия с пользователем.");
            Console.WriteLine("Пользователь должен загадать число в диапазоне от 0 до 100.");
            Console.WriteLine("Компьютер будет пытаться его угадать.");

            while (true)
            {
                PlayGame();

                Console.WriteLine("Сыграть еще? (y/n)");
                string answer = Console.ReadLine();
                if (!answer.Equals("y", StringComparison.OrdinalIgnoreCase))
                    break;
            }
        }

        // Основная игровая логика
        static void PlayGame()
        {
            int low = 0;
            int high = 100;
            bool guessed = false;
            int k = 0;
            while (!guessed)
            {
                k++;
                int guess = (low + high) / 2;

                int result = AskUser(guess);

                if (result == 1)
                {
                    // Загаданное число меньше
                    high = guess - 1;
                }
                else if (result == 2)
                {
                    // Загаданное число больше
                    low = guess + 1;
                }
                else if (result == 0)
                {
                    // Угадано
                    MessageBox(IntPtr.Zero, $"Число {guess} угадано за {k} попыток.", "Результат", 0);
                    guessed = true;
                }
            }
        }

        // Метод для взаимодействия через MessageBox
        static int AskUser(int guess)
        {
            string text = $"Ваше число {guess}?";
            string caption = "Угадывание числа";

            uint MB_YESNO = 0x00000004;
            uint MB_ICONQUESTION = 0x00000040;

            // Да — угадано, Нет — уточняем дальше
            int response = MessageBox(IntPtr.Zero, text, caption, MB_YESNO | MB_ICONQUESTION);

            if (response == 6) // IDYES
                return 0;

            // Уточняем: число больше?
            uint MB_YESNOCANCEL = 0x00000003;

            int higher = MessageBox(IntPtr.Zero, "Загаданное число больше?", "Уточнение", MB_YESNOCANCEL | MB_ICONQUESTION);

            if (higher == 6)      // Yes
                return 2;

            if (higher == 7)      // No
                return 1;

            // Cancel — выход из программы
            MessageBox(IntPtr.Zero, "Игра прервана пользователем.", "Информация", 0);
            Environment.Exit(0);
            return -1;
        }
    }
}
