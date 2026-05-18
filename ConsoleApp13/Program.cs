using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ProcessConfirmationApp
{
    static class WinApiHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public const int IDYES = 6;
        private const uint MB_OK = 0x00000000;
        private const uint MB_YESNO = 0x00000004;
        private const uint MB_ICONINFORMATION = 0x00000040;
        private const uint MB_ICONWARNING = 0x00000030;
        private const uint MB_ICONQUESTION = 0x00000020;

        public static void ShowInfo(string text, string caption) => MessageBox(IntPtr.Zero, text, caption, MB_OK | MB_ICONINFORMATION);
        public static void ShowWarning(string text, string caption) => MessageBox(IntPtr.Zero, text, caption, MB_OK | MB_ICONWARNING);
        public static int ShowYesNo(string text, string caption) => MessageBox(IntPtr.Zero, text, caption, MB_YESNO | MB_ICONQUESTION);
    }

    class Player
    {
        public string Name { get; }
        public int Score { get; set; }
        public Player(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Игрок" : name;
            Score = 0;
        }
    }

    abstract class Quest
    {
        public string Title { get; protected set; }
        public abstract int Execute();

        protected int GetSpeedBonus(TimeSpan elapsed)
        {
            if (elapsed.TotalSeconds < 5)
            {
                Console.WriteLine(" Бонус за скорость: +5 очков!");
                return 5;
            }
            return 0;
        }
    }

    class MathQuest : Quest
    {
        public MathQuest() { Title = "Новогодние воспоминания"; }
        public override int Execute()
        {
            Console.WriteLine("Как звали Иисуса Христа на вымершем языке");
            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Ваш ответ: ");
            string answer = Console.ReadLine();
            sw.Stop();

            if (answer == "Яхве")
            {
                WinApiHelper.ShowInfo("Верно!", Title);
                return 10 + GetSpeedBonus(sw.Elapsed);
            }
            WinApiHelper.ShowWarning("Не верно!", Title);
            return 0;
        }
    }

    class RiddleQuest : Quest
    {
        public RiddleQuest() { Title = "Новогодняя загадка"; }
        public override int Execute()
        {
            Console.WriteLine("Зимой и летом одним цветом.");
            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Ваш ответ: ");
            string answer = Console.ReadLine()?.ToLower();
            sw.Stop();

            if (answer == "елка" || answer == "ёлка" || answer == "ель")
            {
                WinApiHelper.ShowInfo("Верно!", Title);
                return 10 + GetSpeedBonus(sw.Elapsed);
            }
            WinApiHelper.ShowWarning("Неверный ответ.", Title);
            return 0;
        }
    }

    class MemoryQuest : Quest
    {
        public MemoryQuest() { Title = "Испытание на внимательность"; }
        public override int Execute()
        {
            string sequence = "СНЕГ ЕЛКА ПОДАРОК";
            WinApiHelper.ShowInfo(sequence, "Запомните это!");

            Console.WriteLine("Запоминайте слова в окне сообщения...");
            Console.WriteLine("У тебя есть 8 секунд");
            Thread.Sleep(8000);

            Console.Clear();
            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Введите последовательность: ");
            string input = Console.ReadLine()?.ToUpper();
            sw.Stop();

            if (input == sequence)
            {
                WinApiHelper.ShowInfo("Отличная память!", Title);
                return 10 + GetSpeedBonus(sw.Elapsed);
            }
            WinApiHelper.ShowWarning("Ошибка.", Title);
            return 0;
        }
    }

    class FinalChoiceQuest : Quest
    {
        public FinalChoiceQuest() { Title = "Последний выбор"; }
        public override int Execute()
        {
            int result = WinApiHelper.ShowYesNo("Пожертвовать подарками ради спасения праздника?", Title);
            if (result == WinApiHelper.IDYES)
            {
                Console.WriteLine("Вы спасли праздник.");
                return 10;
            }
            return 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int start = WinApiHelper.ShowYesNo("Начать новогодний квест?", "Новогодний квест 2025");
            if (start != WinApiHelper.IDYES) return;

            Console.Clear();
            Console.Write("Введите имя игрока: ");
            Player player = new Player(Console.ReadLine());

            Quest[] quests = { new RiddleQuest(), new MathQuest(), new MemoryQuest(), new FinalChoiceQuest() };

            foreach (Quest quest in quests)
            {
                Console.Clear();
                Console.WriteLine($"Квест: {quest.Title}\n");
                player.Score += quest.Execute();
                Console.WriteLine($"\nТекущий счет: {player.Score}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }

            Console.Clear();
            Console.WriteLine($"Игра завершена, {player.Name}!");
            Console.WriteLine($"Ваш итоговый счёт: {player.Score}");

            WinApiHelper.ShowInfo($"Игра окончена!\nВаш счет: {player.Score}", "Финиш");
        }
    }
}