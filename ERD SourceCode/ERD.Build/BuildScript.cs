using ERD.Base;
using ERD.Build.Models;
using ERD.Common;
using Newtonsoft.Json;
using System.IO;

namespace ERD.Build
{
  public static class BuildScript
  {
    public static BuildSetupModel Setup { get; set;}

    public static void Save()
    {
      if (BuildScript.Setup == null)
      {
        return;
      }

      string buildFile = Path.Combine(General.ProjectModel.FileDirectory, $"{General.ProjectModel.ModelName}.{FileTypes.estp}");

      File.WriteAllText(buildFile, JsonConvert.SerializeObject(BuildScript.Setup));
    }
  }
}
