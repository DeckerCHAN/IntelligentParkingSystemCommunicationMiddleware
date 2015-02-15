using System;

namespace IPSCM.Logging.EventArgs
{
    public class LogErrorEventArgs : System.EventArgs
    {
        private string message;
        private Exception Exception { get; set; }

        public String Message
        {
            get
            {
                if (this.Exception != null)
                {
                    this.message += String.Format("{0}Caused by:{1}", Environment.NewLine, this.Exception.ToString());
                }
                return this.message;
            }
        }

        public LogErrorEventArgs(String message)
        {
            this.message = message;
        }

        public LogErrorEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public LogErrorEventArgs(String message, Exception exception)
        {
            this.message = message;
            this.Exception = exception;
        }


    }
}
