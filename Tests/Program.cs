﻿#region

using System;
using IPSCM.UI;

#endregion

namespace IPSCM.Tests
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var c = new UIControl();
            c.MainWindow.Show();
            c.Run();
            Console.ReadKey();
        }
    }
}