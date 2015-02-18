using System;
using System.Collections.Generic;
using System.IO;

namespace IPSCM.Configuration
{
    public class FileConfig : Configuration.Config
    {
        public static LinkedList<FileConfig> FileConfigs = new LinkedList<FileConfig>();

        private FileInfo configFile;
        public FileConfig(FileInfo configFile)
            : base()
        {
            this.configFile = configFile;
            if (!configFile.Exists)
            {
                throw new FileNotFoundException(String.Format("File {0} not exists!", configFile.Name));
            }
            var line = String.Empty;

            using (var reader = new StreamReader(configFile.OpenRead()))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line)) { continue; }
                    var key = line.Substring(0, line.IndexOf(':'));
                    var value = line.Substring(line.IndexOf(':'));

                    if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
                    {
                        throw new FormatException(String.Format("Encountered a mistake during split line:{0}", line));
                    }
                    this.Set(key, value);
                }
            }

            FileConfigs.AddLast(this);
        }

        public void SaveToFile()
        {
            using (StreamWriter writer = new StreamWriter(this.configFile.OpenWrite()))
            {
                foreach (var key in this.configs.Keys)
                {
                    writer.WriteLine("{0}:{1}", key, this.GetString(key));
                }
                writer.Flush();
            }

        }
    }
}
