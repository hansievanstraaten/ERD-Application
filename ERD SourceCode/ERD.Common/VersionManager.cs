using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using ViSo.Common;

namespace ERD.Common
{
    public class VersionManager
    {
        private readonly string downloadUrl = "https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi/";
        private readonly string versionFile = "VersionFile.txt";
        private readonly string msiFile = "ViSo.Viewer";
        private readonly string msiExstention = ".msi";
        
        public static string ServerVersion { get; set; }

        public bool HaveUpdates(string thisVersion)
        {
            try
            {
                string downloadFile = Path.Combine(this.downloadUrl, this.versionFile);

                string saveVersionFile = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), this.versionFile);

                using(DownloadWebClient client = new DownloadWebClient())
                {
                    client.DownloadFile(downloadFile, saveVersionFile);
                }

                VersionManager.ServerVersion = File.ReadAllText(saveVersionFile)
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty);

                File.Delete(saveVersionFile);

                return thisVersion != VersionManager.ServerVersion;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        public void InstallUpdates()
        {
            try
            {
                string downloadFile = Path.Combine(this.downloadUrl, $"{this.msiFile}{this.msiExstention}");

                string saveVersionFile = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{this.msiFile}.{VersionManager.ServerVersion}{this.msiExstention}");

                using (DownloadWebClient client = new DownloadWebClient())
                {
                    client.DownloadFile(downloadFile, saveVersionFile);
                }

                Process.Start(saveVersionFile);
            }
            catch (Exception err)
            {
                throw;
            }
        }

        private class DownloadWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest result = base.GetWebRequest(address);

                result.Timeout = 60000 * 5; // 5 Minutes


                return result;
            }
        }
    }
}
