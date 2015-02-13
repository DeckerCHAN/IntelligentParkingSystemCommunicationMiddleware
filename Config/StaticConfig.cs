using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{
    class StaticConfig:Config
    {
        private static StaticConfig _instance;

        public static StaticConfig GetConfig()
        {
            return _instance ?? (_instance = new StaticConfig());
        }

        private StaticConfig() : base()
        {
            
        }
    }
}
