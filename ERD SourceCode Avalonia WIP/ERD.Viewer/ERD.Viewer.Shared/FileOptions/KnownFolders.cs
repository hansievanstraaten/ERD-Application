using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ERD.Viewer.Shared.FileOptions
{
    public static class KnownFolders
    {
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static readonly bool IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        // ---------- Windows GUIDs ----------
        private static readonly string[] WindowsKnownFolderGuids =
        {
            "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
            "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
            "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
            "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
            "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
            "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
            "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
            "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
            "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
            "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
            "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
        };

        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            nint hToken,
            out nint ppszPath);

        [Flags]
        private enum KnownFolderFlags : uint
        {
            SimpleIDList = 0x00000100,
            NotParentRelative = 0x00000200,
            DefaultPath = 0x00000400,
            Init = 0x00000800,
            NoAlias = 0x00001000,
            DontUnexpand = 0x00002000,
            DontVerify = 0x00004000,
            Create = 0x00008000,
            NoAppcontainerRedirection = 0x00010000,
            AliasOnly = 0x80000000
        }

        public enum KnownFolder
        {
            Contacts,
            Desktop,
            Documents,
            Downloads,
            Favorites,
            Links,
            Music,
            Pictures,
            SavedGames,
            SavedSearches,
            Videos
        }

        // =============================
        // PUBLIC ACCESSOR
        // =============================
        public static string GetPath(KnownFolder folder) =>
            IsWindows ? GetWindowsPath(folder, false)
          : IsMac ? GetMacPath(folder)
          : IsLinux ? GetLinuxPath(folder)
          : throw new PlatformNotSupportedException("Unknown OS");

        public static string GetPath(KnownFolder folder, bool defaultUser) =>
            IsWindows ? GetWindowsPath(folder, defaultUser)
          : IsMac ? GetMacPath(folder)
          : IsLinux ? GetLinuxPath(folder)
          : throw new PlatformNotSupportedException("Unknown OS");

        // =============================
        // WINDOWS IMPLEMENTATION
        // =============================
        private static string GetWindowsPath(KnownFolder folder, bool defaultUser)
        {
            int result = SHGetKnownFolderPath(
                new Guid(WindowsKnownFolderGuids[(int)folder]),
                (uint)KnownFolderFlags.DontVerify,
                new IntPtr(defaultUser ? -1 : 0),
                out nint outPath);

            if (result >= 0)
            {
                string path = Marshal.PtrToStringUni(outPath);
                Marshal.FreeCoTaskMem(outPath);
                return path;
            }

            throw new ExternalException("Unable to get folder path.", result);
        }

        // =============================
        // MACOS IMPLEMENTATION (POSIX)
        // =============================
        private static string GetMacPath(KnownFolder folder)
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return folder switch
            {
                KnownFolder.Desktop => Path.Combine(home, "Desktop"),
                KnownFolder.Documents => Path.Combine(home, "Documents"),
                KnownFolder.Downloads => Path.Combine(home, "Downloads"),
                KnownFolder.Music => Path.Combine(home, "Music"),
                KnownFolder.Pictures => Path.Combine(home, "Pictures"),
                KnownFolder.Videos => Path.Combine(home, "Movies"),
                KnownFolder.Favorites => Path.Combine(home, "Library", "Favorites"),
                KnownFolder.Contacts => Path.Combine(home, "Library", "Application Support", "AddressBook"),
                KnownFolder.Links => Path.Combine(home, "Library"),
                KnownFolder.SavedGames => Path.Combine(home, "Library", "Saved Games"),
                KnownFolder.SavedSearches => Path.Combine(home, "Library", "Saved Searches"),
                _ => home
            };
        }

        // =============================
        // LINUX IMPLEMENTATION (XDG)
        // =============================
        private static string GetLinuxPath(KnownFolder folder)
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string GetXdgPath(string envName, string fallback) =>
                Environment.GetEnvironmentVariable(envName)
                ?? Path.Combine(home, fallback);

            return folder switch
            {
                KnownFolder.Desktop => GetXdgPath("XDG_DESKTOP_DIR", "Desktop"),
                KnownFolder.Documents => GetXdgPath("XDG_DOCUMENTS_DIR", "Documents"),
                KnownFolder.Downloads => GetXdgPath("XDG_DOWNLOAD_DIR", "Downloads"),
                KnownFolder.Music => GetXdgPath("XDG_MUSIC_DIR", "Music"),
                KnownFolder.Pictures => GetXdgPath("XDG_PICTURES_DIR", "Pictures"),
                KnownFolder.Videos => GetXdgPath("XDG_VIDEOS_DIR", "Videos"),
                KnownFolder.Favorites => Path.Combine(home, ".local", "share"),
                KnownFolder.SavedSearches => Path.Combine(home, ".local", "share", "searches"),
                KnownFolder.SavedGames => Path.Combine(home, ".local", "share", "games"),
                KnownFolder.Links => Path.Combine(home, ".local", "share"),
                KnownFolder.Contacts => home,
                _ => home
            };
        }
    }
}


//using System.Runtime.InteropServices;

//namespace ERD.Viewer.Shared.FileOptions
//{
//  public static class KnownFolders
//  {
//    [DllImport("Shell32.dll")]
//    private static extern int SHGetKnownFolderPath(
//      [MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, nint hToken,
//      out nint ppszPath);

//    private static string[] knownFolderGuids = new string[]
//    {
//      "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
//      "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
//      "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
//      "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
//      "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
//      "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
//      "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
//      "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
//      "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
//      "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
//      "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
//    };

//    [Flags]
//    private enum KnownFolderFlags : uint
//    {
//      SimpleIDList = 0x00000100,
//      NotParentRelative = 0x00000200,
//      DefaultPath = 0x00000400,
//      Init = 0x00000800,
//      NoAlias = 0x00001000,
//      DontUnexpand = 0x00002000,
//      DontVerify = 0x00004000,
//      Create = 0x00008000,
//      NoAppcontainerRedirection = 0x00010000,
//      AliasOnly = 0x80000000
//    }

//    public enum KnownFolder
//    {
//      Contacts,
//      Desktop,
//      Documents,
//      Downloads,
//      Favorites,
//      Links,
//      Music,
//      Pictures,
//      SavedGames,
//      SavedSearches,
//      Videos
//    }

//    internal static string GetPath(KnownFolder knownFolder)
//    {
//      return GetPath(knownFolder, false);
//    }

//    internal static string GetPath(KnownFolder knownFolder, bool defaultUser)
//    {
//      return GetPath(knownFolder, KnownFolderFlags.DontVerify, defaultUser);
//    }

//    private static string GetPath(KnownFolder knownFolder, KnownFolderFlags flags,
//      bool defaultUser)
//    {
//      int result = SHGetKnownFolderPath(new Guid(knownFolderGuids[(int)knownFolder]), (uint)flags, new nint(defaultUser ? -1 : 0), out nint outPath);

//      if (result >= 0)
//      {
//        string path = Marshal.PtrToStringUni(outPath);

//        Marshal.FreeCoTaskMem(outPath);

//        return path;
//      }
//      else
//      {
//        throw new ExternalException("Unable to retrieve the known folder path. It may not be available on this system.", result);
//      }
//    }
//  }
//}
