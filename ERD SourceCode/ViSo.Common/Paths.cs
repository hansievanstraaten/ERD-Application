using System;
using System.IO;
using System.Threading;
using static ViSo.Common.KnownFolders;

namespace ViSo.Common
{
    public static class Paths
    {
        private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        public static bool IsFileLocked(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);

                using (FileStream stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch
            {
                return true;
            }

            return false;
        }

        public static void WaitFileRelease(string fileName)
        {
            KEEPWAITING:

            try
            {
                if (!File.Exists(fileName))
                {
                    return;
                }

                while(Paths.IsFileLocked(fileName))
                {
                    Paths.manualResetEvent.WaitOne(3000);
                }
            }
            catch (Exception err)
            {
                goto KEEPWAITING;
            }
        }

        public static string StartupPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string StartupPathSubDirectory(string directoryName)
        {
            string result = Path.Combine(Paths.StartupPath, directoryName);

            Paths.CreateDirectory(result);

            return result;
        }

        public static string KnownFolder(KnownFolder folder)
        {
            return KnownFolders.GetPath(folder);
        }

        public static string KnownFolder(KnownFolder folder, bool defaultUser)
        {
            return KnownFolders.GetPath(folder, defaultUser);
        }

        public static void CreateDirectory(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
