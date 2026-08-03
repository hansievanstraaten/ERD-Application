using GeneralExtensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ViSo.Common;

namespace ERD.Common
{
    public class VersionManager
    {
        public event EventHandler<UpdateDownloadedEventArgs>? Update_Downloaded;

        private readonly string msiFile = "ViSo.Viewer";
        private readonly string msiExstention = ".msi";
        
        public static string ServerVersion { get; set; }

        public static bool CheckForUpdatesFailed { get; private set; }

        public static string VersionFile
        {
            get
            {
                return "VersionFile.txt";
            }
        }

        public static string VersionFileURL
        {
            get
            {
                string downloadFile = Path.Combine(DownloadUrl, VersionFile);
                return downloadFile;
            }
        }

        public static string DownloadUrl
        {
            get
            {
                //private readonly string downloadUrl = "https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi/";
                //private readonly string downloadUrl = "https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi%20.Net%2010/";
                return "https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi%20.Net%2010/";
            }
        }

        public async Task<bool> HaveUpdatesAsync(string thisVersion)
        {
            try
            {
                CheckForUpdatesFailed = false;

                string saveVersionFile = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), VersionFile);

                DownloadClient downloader = new DownloadClient();
                await downloader.DownloadFileAsync(VersionFileURL, saveVersionFile);

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

        public async Task DownloadAndInstallUpdatesAsyn(Dispatcher dispatcher)
        {
            try
            {
                string downloadFile = Path.Combine(DownloadUrl, $"{this.msiFile}{this.msiExstention}");

                string saveVersionFile = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{this.msiFile}.{VersionManager.ServerVersion}{this.msiExstention}");

                DownloadClient downloader = new DownloadClient();
                await downloader.DownloadFileAsync(downloadFile, saveVersionFile);

                Update_Downloaded?.Invoke(this, new UpdateDownloadedEventArgs(saveVersionFile));
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

            public async Task DownloadFileAsync(string downloadUrl, string saveFilePath)
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
