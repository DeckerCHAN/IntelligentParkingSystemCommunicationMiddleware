#region

using System;

#endregion

namespace IPSCM.Logging.EventArgs
{
    public class LogInfoEventArgs : System.EventArgs
    {
        public LogInfoEventArgs(String messege)
        {
            this.Messege = messege;
        }

        public String Messege { get; private set; }
    }
}