using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPSCM.Utils
{
    public static class DownloadUtils
    {
        public static DownloadProgress DownloadAsync(String url, FileInfo file)
        {
            var progress = new DownloadProgress();

            var client = new WebClient();
            client.DownloadProgressChanged += (i, o) =>
            {
                lock (progress)
                {
                    progress.Progress = o.ProgressPercentage;
                }
            };
            client.DownloadFileCompleted += (i, o) =>
            {
                lock (progress)
                {
                    progress.IsCompleted = true;
                }
            };

            client.DownloadFileAsync(new Uri(url), file.FullName);




            return progress;
        }
        public static void DownloadSync(String url, FileInfo file)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(new Uri(url), file.FullName);
            }
        }
    }



    public class DownloadProgress
    {
        public int Progress { get; set; }
        public Boolean IsCompleted { get; set; }
        public Boolean IsFailure { get; set; }
        public DownloadProgress()
        {
            this.Progress = 0;
            this.IsCompleted = false;
            this.IsFailure = false;
        }
    }
}
