using GeneralExtensions;
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

        public static bool CheckForUpdatesFailed { get; private set; }

        public bool HaveUpdates(string thisVersion)
        {
            try
            {
                CheckForUpdatesFailed = false;

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

                if (!this.IsVersionNumber(VersionManager.ServerVersion))
				{
                    CheckForUpdatesFailed = true;

                    VersionManager.ServerVersion = string.Empty;
                    // We now need to notify the user that the system updates failed.
                    return true;
				}

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
    
        private bool IsVersionNumber(string version)
		{
            if (version.IsNullEmptyOrWhiteSpace())
			{
                return false;
			}

            string[] versionSplit = version.Split('.');

            if (versionSplit.Length != 4)
			{
                return false;
			}

            for(int x = 0; x < 4; ++x)
			{
                if (!versionSplit[x].IsNumeric())
				{
                    return false;
				}
			}

            return true;
		}
    }
}
