using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace NumberGuessingGame
{
    class BaseAppController
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public virtual void StartApplication()
        {

        }

        public virtual void StopApplication()
        {

        }
    }

    class NotepadController : BaseAppController
    {
        private Process pp;

        public override void StartApplication()
        {
            int result = MessageBox(IntPtr.Zero, "Создать чтобы потом погубить Блокнот?", "Запрос", 0x00000004 | 0x00000020);

            if (result == 6)
            {
                if (pp == null || pp.HasExited)
                {
                    pp = Process.Start("notepad.exe");
                    Console.WriteLine("Блокнот запущен.");
                }
                else
                {
                    Console.WriteLine("Блокнот уже запущен.");
                }
            }
        }

        public override void StopApplication()
        {
            if (pp != null && !pp.HasExited)
            {
                int result = MessageBox(IntPtr.Zero, "Ты уверен что хочеш убть его?", "Запрос", 0x00000004 | 0x00000020);

                if (result == 6)
                {
                    pp.Kill(); 
                    Console.WriteLine("Блокнот остановлен.");
                    MessageBox(IntPtr.Zero, "Блокнот убит нееееееееееет блокнот тудутудуду тутутудутууун.", "Инфо", 0x00000040);
                }
            }
            else
            {
                Console.WriteLine("Блокнот не запущен, нечего закрывать.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BaseAppController controller = new NotepadController();
            

            while (true)
            {
                Console.WriteLine("1. Запустить Блокнот");
                Console.WriteLine("2. Остановить Блокнот");
                Console.WriteLine("3. Выход");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        controller.StartApplication();
                        break;
                    case "2":
                        controller.StopApplication();
                        break;
                    case "3":
                        break;
                    default:
                        Console.WriteLine("Неверный ввод");
                        Console.ReadKey();
                        break;
                }

                    Console.WriteLine("Нажмите клавишу");
                    Console.ReadKey();
            }
        }
    }
}
