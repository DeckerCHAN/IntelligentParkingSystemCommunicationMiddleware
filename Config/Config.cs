﻿#region

using System;
using System.Collections.Generic;

#endregion

namespace IPSCM.Configuration
{
    public abstract class Config
    {
        protected Config()
        {
            this.configs = new Dictionary<string, String>();
        }

        protected Dictionary<String, String> configs { get; set; }

        public virtual void Set(String key, String value)
        {
            key = key.ToUpper();
            if (this.configs.ContainsKey(key))
            {
                this.configs.Remove(key);
            }
            this.configs.Add(key, value);
        }

        public virtual String GetString(String key)
        {
            key = key.ToUpper();
            if (!configs.ContainsKey(key))
            {
                throw new KeyNotFoundException(String.Format("\"{0}\" did not found!", key));
            }
            return this.configs[key];
        }

        public virtual UInt32 GetUInt(String key)
        {
            return Convert.ToUInt32(this.GetString(key));
        }

        public virtual Int32 GetInt(String key)
        {
            return Convert.ToInt32(this.GetString(key));
        }

        public virtual Boolean GetBoolean(String key)
        {
            return Convert.ToBoolean(this.GetString(key));
        }

        public virtual Double GetDouble(String key)
        {
            return Convert.ToDouble(this.GetString(key));
        }
    }
}