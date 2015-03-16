using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IPSCM.Core;
using IPSCM.Logging;
using IPSCM.UI;

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
                var e = Engine.GetEngine();
                e.Run();
            }
            catch (Exception ex)
            {

                FileInfo report =
     new FileInfo(String.Format("crash-report-{0}.report",
         DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")));
                File.WriteAllText(report.FullName, ex.ToString());
            }

        }

        static void Program_OnError(IPSCM.Logging.EventArgs.LogErrorEventArgs e)
        {
            Console.WriteLine("[Error]" + e.Message);
        }

        static void Program_OnInfo(IPSCM.Logging.EventArgs.LogInfoEventArgs e)
        {
            Console.WriteLine("[Info]"+e.Messege);
        }
    }
}
