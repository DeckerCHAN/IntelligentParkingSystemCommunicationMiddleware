using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPSCM.Config
{
    class FileSyncConfig:FileConfig
    {
        public FileSyncConfig(FileInfo configFile) : base(configFile)
        {
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);
            this.SaveToFile();
        }
    }
}
