using System;
using System.Collections.Generic;
using System.IO;

namespace IPSCM.Configuration
{
    public class FileConfig : Config
    {
        public static Dictionary<String, FileConfig> FileConfigs = new Dictionary<String, FileConfig>();

        public static FileConfig FindConfig(String name)
        {
            //TODO:Using better implantation
            if (FileConfigs.ContainsKey(name))
            {
                return FileConfigs[name];
            }
            var file = new FileInfo("Configs\\" + name);
            return new FileConfig(file);
        }

        private readonly FileInfo ConfigFile;
        protected FileConfig(FileInfo configFile)
        {
            this.ConfigFile = configFile;
            if (!configFile.Exists)
            {
                throw new FileNotFoundException(String.Format("File {0} not exists!", configFile.Name));
            }
            var line = String.Empty;

            using (var reader = new StreamReader(configFile.OpenRead()))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line) || line[0] == '#'||!line.Contains(":")) { continue; }
                    var key = line.Substring(0, line.IndexOf(':'));
                    var value = line.Substring(line.IndexOf(':') + 1);

                    if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
                    {
                        throw new FormatException(String.Format("Encountered a mistake during split line:{0}", line));
                    }
                    this.Set(key, value);
                }
            }

            FileConfigs.Add(configFile.Name, this);
        }

        public void SaveToFile()
        {
            using (var writer = new StreamWriter(this.ConfigFile.OpenWrite()))
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
