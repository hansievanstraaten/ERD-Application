using ERD.Models;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace ERD.Common
{
    public static class General
    {
        public static ProjectModel ProjectModel { get; set; }

        /// <summary>
        /// Get product/version information by product name, executable path or empty to return current app version.
        /// Uses registry uninstall keys and file version info. Avoids Win32_Product.
        /// </summary>
        public static string GetProductVersion(string softWareName)
        {
            try
            {
                string version = SearchUninstallRegistry(softWareName);

                if (!string.IsNullOrEmpty(version))
                {
                    return version;
                }

                return "Product/version not found";
            }
            catch (Exception ex)
            {
                // Do not rethrow to avoid breaking callers; return descriptive text for logging.
                return $"Error retrieving version: {ex.Message}";
            }
        }

        private static string GetVersionFromFile(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
                var info = FileVersionInfo.GetVersionInfo(path);
                return info.ProductVersion ?? info.FileVersion;
            }
            catch
            {
                return null;
            }
        }

        private static string SearchUninstallRegistry(string softWareName)
        {
            try
            {
                string[] uninstallSubKeys = new[]
                {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
                };

                // Search HKLM in both views and HKCU default view
                // First HKLM 64-bit view
                foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
                {
                    using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                    {
                        foreach (var subKeyPath in uninstallSubKeys)
                        {
                            using (var uninstallKey = baseKey.OpenSubKey(subKeyPath))
                            {
                                string result = SearchUninstallKeyForName(uninstallKey, softWareName);
                                if (!string.IsNullOrEmpty(result)) return result;
                            }
                        }
                    }
                }

                // Then HKCU (current user)
                using (var cu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
                using (var uninstallKey = cu.OpenSubKey(uninstallSubKeys[0]))
                {
                    string result = SearchUninstallKeyForName(uninstallKey, softWareName);
                    if (!string.IsNullOrEmpty(result)) return result;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string SearchUninstallKeyForName(RegistryKey uninstallKey, string nameToMatch)
        {
            if (uninstallKey == null) return null;

            foreach (var subKeyName in uninstallKey.GetSubKeyNames())
            {
                using (var sub = uninstallKey.OpenSubKey(subKeyName))
                {
                    if (sub == null) continue;
                    try
                    {
                        var displayName = sub.GetValue("DisplayName") as string;
                        if (string.IsNullOrEmpty(displayName)) continue;

                        if (displayName.IndexOf(nameToMatch, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var displayVersion = sub.GetValue("DisplayVersion") as string;
                            if (!string.IsNullOrEmpty(displayVersion)) return displayVersion;

                            var version = sub.GetValue("Version") as string;
                            if (!string.IsNullOrEmpty(version)) return version;

                            // If installer provides an InstallLocation or UninstallString, try to fetch exe file version
                            var installLoc = sub.GetValue("InstallLocation") as string;
                            if (!string.IsNullOrEmpty(installLoc))
                            {
                                var exe = Directory.EnumerateFiles(installLoc, "*.exe", SearchOption.TopDirectoryOnly)
                                                   .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f)
                                                   .IndexOf(Path.GetFileNameWithoutExtension(nameToMatch), StringComparison.OrdinalIgnoreCase) >= 0);
                                if (!string.IsNullOrEmpty(exe))
                                {
                                    var fv = GetVersionFromFile(exe);
                                    if (!string.IsNullOrEmpty(fv)) return fv;
                                }
                            }

                            var uninstallStr = sub.GetValue("UninstallString") as string;
                            if (!string.IsNullOrEmpty(uninstallStr))
                            {
                                // try to extract exe path
                                var possible = uninstallStr.Trim('"');
                                var exePath = possible.Split(new[] { ".exe" }, StringSplitOptions.None).FirstOrDefault();
                                if (!string.IsNullOrEmpty(exePath))
                                {
                                    exePath = exePath + ".exe";
                                    if (File.Exists(exePath))
                                    {
                                        var fv = GetVersionFromFile(exePath);
                                        if (!string.IsNullOrEmpty(fv)) return fv;
                                    }
                                }
                            }

                            // matched name but no version info found
                            return "Installed (version unknown)";
                        }
                    }
                    catch
                    {
                        // ignore and continue
                    }
                }
            }

            return null;
        }
    }
}
