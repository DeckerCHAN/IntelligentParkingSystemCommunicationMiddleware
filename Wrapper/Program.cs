using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using IPSCM.Core;
using IPSCM.Logging;
using IPSCM.Logging.EventArgs;

namespace Wrapper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Log.GetLogger().OnInfo += Program_OnInfo;
                Log.GetLogger().OnError += Program_OnError;

                if (Process.GetProcessesByName("IPSCM").Length > 1)
                {
                    MessageBox.Show("IPSCM is running!", "Error!");
                    Environment.Exit(-1);
                }

                var e = Engine.GetEngine();
                e.Run();
            }
            catch (Exception ex)
            {

                FileInfo report =
                    new FileInfo(String.Format("crash-report-{0}.report",
                        DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")));
                File.WriteAllText(report.FullName, ex.ToString());
                Console.WriteLine("!!!CRASH!!!Encountered unhandled exception. System crashed!");
                Console.WriteLine("!!!CRASH!!!More detail at {0}", report.FullName);
            }
            finally
            {
                Console.WriteLine("Programme returned...Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

        }

        static void Program_OnError(LogErrorEventArgs e)
        {
            Console.WriteLine("[Error]" + e.Message);
        }

        static void Program_OnInfo(LogInfoEventArgs e)
        {
            Console.WriteLine("[Info]" + e.Messege);
        }
    }
}
