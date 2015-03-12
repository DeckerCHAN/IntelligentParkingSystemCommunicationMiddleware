#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace IPSCM.GUI
{
    public partial class MainConsoleForm : Form
    {
        public MainConsoleForm()
        {
            InitializeComponent();
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
            if ((this.ConsoleTextBox.MaxLength - this.ConsoleTextBox.TextLength) <= (this.ConsoleTextBox.MaxLength/5))
            {
                this.ConsoleTextBox.Clear();
            }
            this.ConsoleTextBox.SelectionStart = this.ConsoleTextBox.TextLength;
            this.ConsoleTextBox.SelectionLength = 0;

            this.ConsoleTextBox.SelectionColor = color;
            this.ConsoleTextBox.AppendText(text);
            this.ConsoleTextBox.SelectionColor = this.ConsoleTextBox.ForeColor;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //this.ShowDialog(UiControl.GetUiControl().LoginWindow);
        }
    }
}