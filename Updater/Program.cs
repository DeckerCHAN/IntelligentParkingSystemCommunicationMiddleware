using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using IPSCM.Updater;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    throw new ArgumentException("Wrong argument number!");
                }

                var currentVersion = args[0];
                var targetVersion = args[1];
                var downloadUrl = args[2];
                var updateFileInfo = new FileInfo("update\\update.zip");
                var updateDirectoryInfo = new DirectoryInfo("update");
                var tempDirectoryInfo = new DirectoryInfo("temp");
                var currentDirectonryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

                Console.WriteLine("Trying update from version:{0} to {1}", currentVersion, targetVersion);

                if (updateDirectoryInfo.Exists)
                { updateDirectoryInfo.Delete(true); }
                updateDirectoryInfo.Create();

                if (tempDirectoryInfo.Exists)
                { tempDirectoryInfo.Delete(true); }
                tempDirectoryInfo.Create();

                Utils.DownloadSync(downloadUrl, updateFileInfo);
                Console.WriteLine("File downloaded");
                Utils.ExtractZipFile(updateFileInfo.FullName, String.Empty, tempDirectoryInfo.FullName);
                Console.WriteLine("Waiting for IPSCM exit...");
                var processes = Process.GetProcessesByName("IPSCM");
                foreach (var process in processes)
                {
                    process.WaitForExit();
                }
                Console.WriteLine("IPSCM exited...start to update!");
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(tempDirectoryInfo.FullName, "*",
                    SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(tempDirectoryInfo.FullName, currentDirectonryInfo.FullName));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(tempDirectoryInfo.FullName, "*.*",
                    SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Copy(newPath, newPath.Replace(tempDirectoryInfo.FullName, currentDirectonryInfo.FullName), true);
                        Console.WriteLine("Successful updated:{0}", newPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failure updated:{0}", newPath);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Update exit!");
                Console.ReadKey();
            }

        }
    }
}
