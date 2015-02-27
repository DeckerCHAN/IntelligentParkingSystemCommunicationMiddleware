﻿using System;
using System.IO;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Logging.EventArgs;

namespace IPSCM.Logging
{
    public delegate void LogInfoEventHandler(LogInfoEventArgs e);

    public delegate void LogErrorEventHandler(LogErrorEventArgs e);
    public class Log
    {

        public  event LogInfoEventHandler OnInfo;
        public  event LogErrorEventHandler OnError;
        private static Log Instance;

        public static Log GetLogger()
        {
            if (Instance == null)
            {
                Instance = new Log();
            }
            return Instance;
        }

        public static void Info(String message)
        {
            var trigger = Log.GetLogger().OnInfo;

            if (trigger != null) trigger(new LogInfoEventArgs(message.Clone().ToString()));


        }

        public static void Error(String message)
        {
            var trigger = Log.GetLogger().OnError;
            if (trigger != null) trigger(new LogErrorEventArgs(message.Clone().ToString()));


        }

        public static void Error(String message, Exception exception)
        {
            var trigger = Log.GetLogger().OnError;
            if (trigger != null) trigger(new LogErrorEventArgs(message.Clone().ToString(), exception));

        }

        private StreamWriter LogFileStreamWriter;
        private Log()
        {
            FileInfo logFile = new FileInfo(StaticConfig.GetConfig().GetString("LogPath"));
            if (!Directory.GetParent(logFile.ToString()).Exists)
            {
                Directory.GetParent(logFile.ToString()).Create();
            }
            LogFileStreamWriter = new StreamWriter(StaticConfig.GetConfig().GetString("LogPath"));
            OnInfo += Log_OnInfo;
            OnError += Log_OnError;
        }

        void Log_OnError(LogErrorEventArgs e)
        {
           LogFileStreamWriter.WriteLine("[{0}] ERROR:{1}", DateTime.Now.ToString("O"), e.Message);
            LogFileStreamWriter.Flush();
        }

        void Log_OnInfo(LogInfoEventArgs e)
        {
            LogFileStreamWriter.WriteLine("[{0}] INFO:{1}", DateTime.Now.ToString("O"), e.Messege);
            LogFileStreamWriter.Flush();
        }
    }
}
