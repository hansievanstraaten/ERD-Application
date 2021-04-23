using ERD.Models;
using System;
using System.Management;

namespace ERD.Common
{
    public static class General
    {
        public static ProjectModel ProjectModel { get; set; }

        public static string GetProductVersion(string softWareName)
        {
            string strVersion = string.Empty;
            try
            {
                object version = (object)null;
                
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product where Name LIKE '%" + softWareName + "%'");

                foreach (ManagementObject obj in searcher.Get())
                {
                    version = obj["Version"];

                    break;
                }

                if (version != null)
                {
                    strVersion = (string)version;
                }
                else
                {
                    strVersion = "Given Product is not found the list of Installed Programs";
                }
            }
            catch (Exception err)
            {
                throw;
            }

            return strVersion;
        }
    }
}
