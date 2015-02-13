using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{

    public abstract class Config
    {
        private Dictionary<String, Object> configs { get; set; }

        protected Config()
        {
            this.configs = new Dictionary<string, Object>();
        }

        public void Set(String key, Object value)
        {
            key = key.ToUpper();
            this.configs.Add(key, value);
        }

        public Object Get(String key)
        {
            key = key.ToUpper();
            return this.configs[key];
        }

        public String GetString(String key)
        {
            key = key.ToUpper();
            return this.configs[key] as String;
        }

        public UInt32 GetUInt(String key)
        {
            key = key.ToUpper();
            return Convert.ToUInt32(this.configs[key]);
        }
    }
}
