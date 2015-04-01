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
            var progress = DownloadUtils.DownloadAsync("http://dlsw.baidu.com/sw-search-sp/soft/3a/12350/QQ_V7.0.14275.0_setup.1426647314.exe", new FileInfo(@"G:\CSharp\IntelligentParkingSystemCommunicationMiddleware\Tests\bin\Debug\QQ.exe"));
            do
            {
                Console.WriteLine(progress.Progress + "%");
                Thread.Sleep(100);
            } while (!progress.IsCompleted);
            Console.ReadKey();
        }
    }
}
