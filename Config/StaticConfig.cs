using System;

namespace IPSCM.Config
{
    public sealed class StaticConfig : Config
    {
        private static StaticConfig _instance;

        public static StaticConfig GetConfig()
        {
            return _instance ?? (_instance = new StaticConfig());
        }

        private StaticConfig() : base()
        {
            this.Set("LogPath","Logs\\"+DateTime.Now.ToString("F").Replace(':','-'));
            this.Set("UserName","TestUser");
            this.Set("UserPassword","defaultPassword");
            
        }
    }
}
