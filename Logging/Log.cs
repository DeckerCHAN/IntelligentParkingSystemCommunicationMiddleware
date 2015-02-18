using System;
using System.IO;
using IPSCM.Configuration;
using IPSCM.Logging.EventArgs;

namespace IPSCM.Logging
{
    public delegate void LogInfoEventHandler(LogInfoEventArgs e);

    public delegate void LogErrorEventHandler(LogErrorEventArgs e);
    public static class Log
    {
        private static Boolean _isInitialized = false;
        private static StreamWriter _logFileStreamWriter;
        public static event LogInfoEventHandler OnInfo;
        public static event LogErrorEventHandler OnError;

        private static void Init()
        {
            if (!_isInitialized)
            {
                FileInfo logFile = new FileInfo(StaticConfig.GetConfig().GetString("LogPath"));
                if (!Directory.GetParent(logFile.ToString()).Exists)
                {
                    Directory.GetParent(logFile.ToString()).Create();
                }
                _logFileStreamWriter = new StreamWriter(StaticConfig.GetConfig().GetString("LogPath"));
                _logFileStreamWriter.AutoFlush = true;
                _isInitialized = true;
            }
        }

        public static void Info(String message)
        {
            Init();
            _logFileStreamWriter.WriteLine("[{0}] INFO:{1}", DateTime.Now.ToString("O"), message);
            if (OnInfo != null) OnInfo(new LogInfoEventArgs(message));
        }

        public static void Error(String message)
        {
            Init();
            _logFileStreamWriter.WriteLine("[{0}] ERROR:{1}", DateTime.Now.ToString("O"), message);
            if (OnError != null) OnError(new LogErrorEventArgs(message));
        }

        public static void Error(String message, Exception exception)
        {
            Init();
            _logFileStreamWriter.WriteLine("[{0}] ERROR:{1}", DateTime.Now.ToString("O"), String.Format("{0} {1} Detail:{2}", message, Environment.NewLine, exception));

            if (OnError != null) OnError(new LogErrorEventArgs(message,exception));
        }


    }
}
