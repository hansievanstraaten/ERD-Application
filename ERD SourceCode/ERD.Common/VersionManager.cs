using GeneralExtensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ViSo.Common;

namespace ERD.Common
{
    public class VersionManager
    {
        //private readonly string downloadUrl = "https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi/";
        private readonly string downloadUrl = "https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi%20.Net%2010";
        private readonly string versionFile = "VersionFile.txt";
        private readonly string msiFile = "ViSo.Viewer";
        private readonly string msiExstention = ".msi";
        
        public static string ServerVersion { get; set; }

        public static bool CheckForUpdatesFailed { get; private set; }

        public async Task<bool> HaveUpdates(string thisVersion)
        {
            try
            {
                CheckForUpdatesFailed = false;

                string downloadFile = Path.Combine(this.downloadUrl, this.versionFile);

                string saveVersionFile = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), this.versionFile);

                DownloadClient downloader = new DownloadClient();
                await downloader.DownloadFile(downloadFile, saveVersionFile);

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

        public async Task InstallUpdates()
        {
            try
            {
                string downloadFile = Path.Combine(this.downloadUrl, $"{this.msiFile}{this.msiExstention}");

                string saveVersionFile = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{this.msiFile}.{VersionManager.ServerVersion}{this.msiExstention}");

                DownloadClient downloader = new DownloadClient();
                await downloader.DownloadFile(downloadFile, saveVersionFile);

                Process.Start(new ProcessStartInfo
                {
                    FileName = saveVersionFile,
                    UseShellExecute = true
                });
            }
            catch (Exception err)
            {
                throw;
            }
        }

        private class DownloadClient
        {
            private readonly HttpClient _client;

            public DownloadClient()
            {
                _client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
            }

            public async Task<byte[]> DownloadAsync(Uri address)
            {
                return await _client.GetByteArrayAsync(address);
            }

            public async Task DownloadFile(string downloadUrl, string saveFilePath)
            {
                byte[] data = await this.DownloadAsync(new Uri(downloadUrl));
                File.WriteAllBytes(saveFilePath, data);
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
