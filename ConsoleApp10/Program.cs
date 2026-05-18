using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleApp10
{
    class BaseWindowClouser
    {
        public virtual void CloseTarget()
        { }
    }

    class Notepedclose : BaseWindowClouser
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_CLOSE = 0x0010;

        public override void CloseTarget()
        {
            IntPtr hWnd = FindWindow("Notepad", null);

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("Окно не найдено: NO-NO-NO MISTER FISH");
                return;
            }

            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Поиск и закрытие Блокнота...");

            try
            {
                Notepedclose myobject = new Notepedclose();
                myobject.CloseTarget();
                Console.WriteLine("Работа завершена");
            }
            catch 
            {
                Console.WriteLine("ХЗ чё произошло, но мы попытаемся это исправить");
            }

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}