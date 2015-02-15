using System;

namespace IPSCM.Logging.EventArgs
{
    public class LogInfoEventArgs : System.EventArgs
    {
        public String Messege { get; private set; }

        public LogInfoEventArgs(String messege)
        {
            this.Messege = messege;
        }
    }
}
