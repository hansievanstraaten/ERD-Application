using System;
using System.Configuration;

namespace ViSo.Common
{
  public static class Client
  {
    public static string ServerIp
    {
      get
      {
        Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        return appConfig.AppSettings.Settings["ServerIp"].Value;
      }

      set
      {
        Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if (appConfig.AppSettings.Settings["ServerIp"] == null)
        {
          appConfig.AppSettings.Settings.Add(new KeyValueConfigurationElement("ServerIp", value));
        }
        else
        {
          appConfig.AppSettings.Settings["ServerIp"].Value = Convert.ToString(value);
        }

        appConfig.AppSettings.SectionInformation.ForceSave = true;

        appConfig.Save(ConfigurationSaveMode.Full);

        ConfigurationManager.RefreshSection("appSettings");
      }
    }
  }
}
