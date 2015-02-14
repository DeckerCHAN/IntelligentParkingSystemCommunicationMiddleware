using System;
using System.Collections.Generic;

namespace IPSCM.Config
{

    public abstract class Config
    {
        protected Dictionary<String, String> configs { get; set; }

        protected Config()
        {
            this.configs = new Dictionary<string, String>();
        }

        public virtual void Set(String key, String value)
        {
            key = key.ToUpper();
            this.configs.Add(key, value);
        }


        public virtual String GetString(String key)
        {
            key = key.ToUpper();
            return this.configs[key];
        }

        public virtual UInt32 GetUInt(String key)
        {
            key = key.ToUpper();
            return Convert.ToUInt32(this.configs[key]);
        }


    }
}
