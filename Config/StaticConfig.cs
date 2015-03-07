using System;

namespace IPSCM.Configuration
{
    public sealed class StaticConfig : Config
    {
        private static StaticConfig _instance;

        public static StaticConfig GetConfig()
        {
            return _instance ?? (_instance = new StaticConfig());
        }

        private StaticConfig()
        {
            this.Set("LogPath","Logs\\"+DateTime.Now.ToString("F").Replace(':','-')+".log");
            this.Set("UserName","TestUser");
            this.Set("UserPassword","defaultPassword");
            
        }
    }
}
