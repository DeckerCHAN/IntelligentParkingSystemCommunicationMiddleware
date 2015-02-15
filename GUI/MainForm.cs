using System;
using System.Drawing;
using System.Windows.Forms;

namespace IPSCM.GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ConsoleTextBox_TextChanged(object sender, System.EventArgs e)
        {

        }

        public void Out(String text)
        {
            this.ConsoleTextBox.AppendText(text + Environment.NewLine);
            this.ConsoleTextBox.ScrollToCaret();
        }
        public void Out(String text, Color color)
        {
            this.AppendText(text + Environment.NewLine, color);
            this.ConsoleTextBox.ScrollToCaret();
        }

        private void AppendText(string text, Color color)
        {
            this.ConsoleTextBox.SelectionStart = this.ConsoleTextBox.TextLength;
            this.ConsoleTextBox.SelectionLength = 0;

            this.ConsoleTextBox.SelectionColor = color;
            this.ConsoleTextBox.AppendText(text);
            this.ConsoleTextBox.SelectionColor = this.ConsoleTextBox.ForeColor;
        }

    }
}
