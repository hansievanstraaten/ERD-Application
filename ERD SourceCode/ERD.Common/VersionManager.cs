namespace ERD.Common
{
    public class VersionManager
    {
        private readonly string downloadUrl = "https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi/";
        private readonly string versionFile = "VersionFile.txt";

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

                string serverVersion = File.ReadAllText(saveVersionFile);

                return thisVersion == serverVersion;
            }
            catch (Exception err)
            {
                //<a style="color:#0000FF" href="https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi/VersionFile.txt" download="ViSo.Viewer.msi">Download Application</a>
                return false;
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
