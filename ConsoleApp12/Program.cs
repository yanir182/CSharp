using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcessManagerApp
{
    class BaseController
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public virtual void Exala() { }
        public virtual void Astanovites() { }
        public virtual void Check() { }

        protected bool c(string msg)
        {
            int r = MessageBox(IntPtr.Zero, msg, "Запрос", 0x00000004 | 0x00000020);
            return r == 6;
        }

        protected void N(string msg)
        {
            MessageBox(IntPtr.Zero, msg, "Инфо", 0x00000040);
        }
    }

    class NotepadController : BaseController
    {
        private Process _proc;
        private string _name = "notepad.exe";

        public override void Exala()
        {
            if (c("Реально хочешь запустить Блокнот?"))
            {
                if (_proc == null || _proc.HasExited)
                {
                    _proc = Process.Start(_name);
                    Console.WriteLine("Блокнот ожил.");
                }
                else
                {
                    Console.WriteLine("Он уже и так запущен.");
                }
            }
        }

        public override void Astanovites()
        {
            if (_proc != null && !_proc.HasExited)
            {
                if (c("Убить Блокнот?"))
                {
                    _proc.Kill();
                    Console.WriteLine("Блокнот уничтожен.");
                    N("Процесс успешно завершен.");
                }
            }
            else
            {
                Console.WriteLine("Некого убивать, Блокнот не запущен.");
            }
        }

        public override void Check()
        {
            if (c("Проверить состояние?"))
            {
                bool active = (_proc != null && !_proc.HasExited);
                string stat;
                if (active)
                {
                    stat = "Блокнот в деле";
                }
                else
                {
                    stat = "Блокнот спит";
                }
                Console.WriteLine("Статус: " + stat);
                N(stat);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            BaseController cont = new NotepadController();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Запустить Блокнот");
                Console.WriteLine("2. Завершить Блокнот");
                Console.WriteLine("3. Проверить состояние");
                Console.WriteLine("4. Выход");
                Console.Write("\nТвой выбор: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": cont.Exala(); break;
                    case "2": cont.Astanovites(); break;
                    case "3": cont.Check(); break;
                    case "4": return;
                    default: Console.WriteLine("Нет такого пункта"); break;
                }

                Console.WriteLine("\nЖми кнопку...");
                Console.ReadKey();
            }
        }
    }
}
