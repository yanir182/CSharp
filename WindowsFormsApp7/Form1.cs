using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void правкаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void главноеМенюToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialogMain_FileOk(object sender, CancelEventArgs e)
        {
          
        }

      

        private void правкаToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
                richTextBoxMain.Clear(); 
                this.Text = "Новый документ - Текстовый редактор";
               
        }
    }
}
