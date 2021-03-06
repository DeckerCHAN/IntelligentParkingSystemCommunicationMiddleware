﻿#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace IPSCM.Configuration
{
    public class FileConfig : Config
    {
        public static Dictionary<String, FileConfig> FileConfigs = new Dictionary<String, FileConfig>();
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
                    if (String.IsNullOrEmpty(line) || line[0] == '#' || !line.Contains(":"))
                    {
                        continue;
                    }
                    var key = line.Substring(0, line.IndexOf(':'));
                    var value = line.Substring(line.IndexOf(':') + 1);

                    if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
                    {
                        throw new FormatException(String.Format("Encountered a mistake during split file {0} line:{1}",ConfigFile.Name, line));
                    }
                    this.Set(key, value);
                }
            }

            FileConfigs.Add(configFile.Name, this);
        }

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
    }
}