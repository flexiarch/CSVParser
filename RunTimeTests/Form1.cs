using CSVParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunTimeTests
{
    public partial class Form1 : Form
    {
        List<string> lines = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var line = new SplitLine(textBox1.Text);
            label1.Text = line.Process() ? "OK" : "Źle";
            listBox1.DataSource = line.Result;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = ((ListBox)sender).Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lines.Add(textBox1.Text);
            listBox1.DataSource = lines.ToArray();
        }
    }
}
