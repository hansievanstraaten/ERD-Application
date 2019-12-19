using System;
using System.IO;
using static ViSo.Common.KnownFolders;

namespace ViSo.Common
{
  public static class Paths
  {
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
