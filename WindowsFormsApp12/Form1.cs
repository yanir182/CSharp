using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApp12
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            myProcess = new Process();
            myProcess.StartInfo = new ProcessStartInfo(@"C:\Users\Ученик_9\AppData\Local\Roblox\Versions\version-e380c8edc8f6477c");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myProcess.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myProcess.CloseMainWindow();
            myProcess.Close();
        }
    }
}
