#region

using System;

#endregion

namespace IPSCM.Configuration
{
    public sealed class StaticConfig : Config
    {
        private static StaticConfig _instance;

        private StaticConfig()
        {
            this.Set("LogPath", "Logs\\" + DateTime.Now.ToString("F").Replace(':', '-') + ".log");
        }

        public static StaticConfig GetConfig()
        {
            return _instance ?? (_instance = new StaticConfig());
        }
    }
}