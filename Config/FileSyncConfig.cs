using System.IO;

namespace IPSCM.Configuration
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
