using System;
using System.Data;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using IPSCM.Persistence;
using IPSCM.UI;
using IPSCM.Utils;


namespace Tests
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            new UpdateCheckWindow().Show();
            Console.ReadKey();
        }
    }
}
