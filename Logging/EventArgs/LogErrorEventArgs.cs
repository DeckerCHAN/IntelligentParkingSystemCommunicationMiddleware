﻿#region

using System;

#endregion

namespace IPSCM.Logging.EventArgs
{
    public class LogErrorEventArgs : System.EventArgs
    {
        private string _message;

        public LogErrorEventArgs(String message)
        {
            this._message = message;
        }

        public LogErrorEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public LogErrorEventArgs(String message, Exception exception)
        {
            this._message = message;
            this.Exception = exception;
        }

        public Exception Exception { get; private set; }

        public String Message
        {
            get
            {
                if (this.Exception != null)
                {
                    this._message += String.Format("{0}Caused by:{1}", Environment.NewLine, this.Exception);
                }
                return this._message;
            }
        }
    }
}