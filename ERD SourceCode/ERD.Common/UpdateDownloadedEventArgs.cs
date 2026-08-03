using System;

namespace ERD.Common
{
    public class UpdateDownloadedEventArgs : EventArgs
    {
        public string InstallerPath { get; }

        public UpdateDownloadedEventArgs(string installerPath)
        {
            this.InstallerPath = installerPath;
        }
    }
}
