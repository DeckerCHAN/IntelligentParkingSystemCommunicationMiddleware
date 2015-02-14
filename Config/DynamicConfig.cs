using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPSCM.Config
{
    public class DynamicConfig : IPSCM.Config.Config
    {
        public DynamicConfig(FileInfo configFile)
            : base()
        {
            if (!configFile.Exists)
            {
                throw new IOException(String.Format("File {0} not exists!", configFile.Name));
            }
            var line = String.Empty;
            while ((line = new StreamReader(configFile.OpenRead()).ReadLine()) != null)
            {
                var seg = line.Split(':');
                if (seg.Length != 2)
                {
                    throw new FormatException(String.Format("Encountered a mistake during split line:{0}", line));
                }
                this.Set(line.Split(':')[0], line.Split(':')[1]);
            }
        }
    }
}
