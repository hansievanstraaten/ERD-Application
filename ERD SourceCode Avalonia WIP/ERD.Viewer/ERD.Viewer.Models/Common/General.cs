using ERD.Viewer.Models.ProjectModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ERD.Viewer.Models.Common
{
    public static class General
    {
        public static ProjectModel? ProjectModel { get; set; }

        public static string GetProductVersion(string softwareName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetWindowsVersion(softwareName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return GetMacOSVersion(softwareName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return GetLinuxVersion(softwareName);

            return "Given Product is not found in the list of Installed Programs";
        }

        // -------------------------------------------------------------
        // WINDOWS IMPLEMENTATION
        // -------------------------------------------------------------
        private static string GetWindowsVersion(string softwareName)
        {
            string[] uninstallPaths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (RegistryKey hive in new[] { Registry.LocalMachine, Registry.CurrentUser })
            {
                foreach (string? path in uninstallPaths)
                {
                    using RegistryKey? key = hive.OpenSubKey(path);
                    if (key == null) continue;

                    foreach (string? subkeyName in key.GetSubKeyNames())
                    {
                        using RegistryKey? sub = key.OpenSubKey(subkeyName);
                        if (sub == null) continue;

                        string? displayName = sub.GetValue("DisplayName") as string;
                        if (string.IsNullOrEmpty(displayName)) continue;

                        if (displayName.IndexOf(softwareName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string? version = sub.GetValue("DisplayVersion") as string;
                            return version ?? "Version not available";
                        }
                    }
                }
            }

            return string.Empty;
        }

        // -------------------------------------------------------------
        // MACOS IMPLEMENTATION
        // -------------------------------------------------------------
        private static string GetMacOSVersion(string softwareName)
        {
            string applicationsPath = "/Applications";

            if (!Directory.Exists(applicationsPath))
                return "Not installed";

            foreach (var app in Directory.GetDirectories(applicationsPath, "*.app"))
            {
                if (!app.Contains(softwareName, StringComparison.OrdinalIgnoreCase))
                    continue;

                string infoPlistPath = Path.Combine(app, "Contents", "Info.plist");
                if (!File.Exists(infoPlistPath))
                    continue;

                var plistData = PlistParser.ReadPlist(infoPlistPath);

                if (plistData.TryGetValue("CFBundleShortVersionString", out var version))
                    return version;

                if (plistData.TryGetValue("CFBundleVersion", out var altVersion))
                    return altVersion;
            }

            return "Not installed";
        }

        private static class PlistParser
        {
            public static Dictionary<string, string> ReadPlist(string path)
            {
                var xml = File.ReadAllText(path);
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);

                var dict = new Dictionary<string, string>();
                var keys = doc.GetElementsByTagName("key")
                              .Cast<System.Xml.XmlNode>();

                foreach (var key in keys)
                {
                    string keyName = key.InnerText;
                    var nextNode = key.NextSibling;

                    while (nextNode != null && nextNode.NodeType == System.Xml.XmlNodeType.Whitespace)
                        nextNode = nextNode.NextSibling;

                    if (nextNode != null)
                        dict[keyName] = nextNode.InnerText;
                }

                return dict;
            }
        }

        // -------------------------------------------------------------
        // LINUX IMPLEMENTATION
        // -------------------------------------------------------------
        private static string GetLinuxVersion(string softwareName)
        {
            // Try dpkg-based
            string dpkg = Exec("dpkg", $"-l | grep -i {softwareName}");
            if (!string.IsNullOrWhiteSpace(dpkg))
            {
                var parts = dpkg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                    return parts[2];  // column 3 is the version
            }

            // Try rpm-based
            string rpm = Exec("rpm", $"-qa | grep -i {softwareName}");
            if (!string.IsNullOrWhiteSpace(rpm))
            {
                var dashIndex = rpm.LastIndexOf('-');
                if (dashIndex > 0)
                    return rpm[(dashIndex + 1)..];
            }

            // Try flatpak
            string flatpak = Exec("flatpak", $"list --app --columns=name,version | grep -i {softwareName}");
            if (!string.IsNullOrWhiteSpace(flatpak))
            {
                var parts = flatpak.Split('\t');
                if (parts.Length > 1)
                    return parts[1];
            }

            // Try snap
            string snap = Exec("snap", $"list | grep -i {softwareName}");
            if (!string.IsNullOrWhiteSpace(snap))
            {
                var parts = snap.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    return parts[1];
            }

            return "Not installed";
        }

        // -------------------------------------------------------------
        // HELPER: run shell commands safely
        // -------------------------------------------------------------
        private static string Exec(string cmd, string args)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{cmd} {args}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var p = Process.Start(psi);
                p.WaitForExit(1000);

                return p.StandardOutput.ReadToEnd().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
