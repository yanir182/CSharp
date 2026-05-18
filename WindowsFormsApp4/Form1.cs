using System;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double sum;
            int period;
            double percent;
            double profit;

          
            if (!double.TryParse(textBox1.Text, out sum))
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму.");
                return;
            }

           
            if (!int.TryParse(textBox2.Text, out period))
            {
                MessageBox.Show("Пожалуйста, введите корректный срок в месяцах.");
                return;
            }

            
            if (sum < 10000)
                percent = 3.5;
            else
                percent = 4.5;

            
            profit = sum * (percent / 100 / 12) * period;

            
            label3.Text =
                "Процентная ставка: " + percent.ToString("n") + "%\n" +
                "Доход: " + profit.ToString("C");
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void label1_Click(object sender, EventArgs e) { }

        private void textBox1_TextChanged_1(object sender, EventArgs e) { }

        private void label3_Click(object sender, EventArgs e) { }

        private void Form1_Load(object sender, EventArgs e) { }

      
    }
}