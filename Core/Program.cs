using System;
using System.IO;
using System.Windows.Forms;

namespace IPSCM.Core
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Engine.GetEngine().Run();
            }
            catch(Exception ex)
            {
                FileInfo report = new FileInfo(String.Format("crash-report-{0}.report", DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")));
                File.WriteAllText(report.FullName, ex.ToString());
            }

        }
    }
}
