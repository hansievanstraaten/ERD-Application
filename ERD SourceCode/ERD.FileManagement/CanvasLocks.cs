using ERD.Base;
using ERD.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WPF.Tools.Functions;

namespace ERD.FileManagement
{
  public sealed class CanvasLocks
  {
    private static CanvasLocks localInstance = null;
    
    private static readonly object instanceLock = new Object();

    private static readonly object lockMethodObject = new object();

    public static CanvasLocks Instance
    {
      get
      {
        // - Create the <<??Singleton??>>.
        if (localInstance == null) // - Lock ONLY initial creation
        {
          lock (instanceLock) // - Lock to ensure only one thread get it, other(s) wait.
          {
            if (localInstance == null)
            {
              localInstance = new CanvasLocks();
            }
          }
        }

        return localInstance;
      }
    }

    ~CanvasLocks()
    {
      GC.SuppressFinalize(this);
    }

    public string LockedByUser(string modelSegmentControlName)
    {
      if (!File.Exists(this.LockFullFileName))
      {
        return string.Empty;
      }

      try
      {
        this.WaitLockRelease();

        string segmantName = $"{modelSegmentControlName}:";

        lock (lockMethodObject)
        {
          string fileText = File.ReadAllText(this.LockFullFileName);

          string[] lockedFilesArray = fileText.Split(';');

          foreach (string lockLine in lockedFilesArray)
          {
            if (!lockLine.StartsWith(segmantName))
            {
              continue;
            }

            string[] lockInformation = lockLine.Split(':');

            return lockInformation[1];
          }
        }
      }
      finally
      {
        File.Delete(this.FileLockName);
      }

      return string.Empty;
    }

    public List<string> GetLockedFiles()
    {
      List<string> result = new List<string>();

      if (!File.Exists(this.LockFullFileName))
      {
        return result;
      }

      try
      {
        this.WaitLockRelease();

        lock (lockMethodObject)
        {
          string fileText = File.ReadAllText(this.LockFullFileName);

          string[] lockedFilesArray = fileText.Split(';');

          result.AddRange(lockedFilesArray);
        }
      }
      finally
      {
        File.Delete(this.FileLockName);
      }

      return result;
    }

    public void LockFile(string modelSegmentControlName)
    {
      if (!General.ProjectModel.LockCanvasOnEditing)
      {
        return;
      }

      try
      {
        this.WaitLockRelease();

        lock (lockMethodObject)
        {
          StringBuilder result = new StringBuilder();

          if (File.Exists(this.LockFullFileName))
          {
            result.Append(File.ReadAllText(this.LockFullFileName));
          }

          string segmantName = $"{modelSegmentControlName}:";

          if (result.ToString().Contains(segmantName))
          {
            int segmentIndex = result.ToString().IndexOf(segmantName) + segmantName.Length;

            int readToIndex = result.ToString().IndexOf(';', segmentIndex);

            string lockedByUser = result.ToString().Substring(segmentIndex, (readToIndex - segmentIndex));

            if (lockedByUser != Environment.UserName)
            {
              throw new ApplicationException($"This Canvas is locked by {lockedByUser}.");
            }

            return;
          }

          result.Append($"{modelSegmentControlName}:{Environment.UserName};");

          File.WriteAllText(this.LockFullFileName, result.ToString());
        }
      }
      finally
      {
        File.Delete(this.FileLockName);
      }
    }

    public void UnlockFile(string modelSegmentControlName)
    {
      if (!General.ProjectModel.LockCanvasOnEditing)
      {
        return;
      }

      if (!File.Exists(this.LockFullFileName))
      {
        return;
      }

      try
      {
        this.WaitLockRelease();

        lock (lockMethodObject)
        {
          StringBuilder fileText = new StringBuilder();

          fileText.Append(File.ReadAllText(this.LockFullFileName));

          string segmantName = $"{modelSegmentControlName}:";

          if (fileText.ToString().Contains(segmantName))
          {
            int segmentIndex = fileText.ToString().IndexOf(segmantName) + segmantName.Length;

            int readToIndex = fileText.ToString().IndexOf(';', segmentIndex);

            string lockedByUser = fileText.ToString().Substring(segmentIndex, (readToIndex - segmentIndex));

            if (lockedByUser != Environment.UserName)
            {
              throw new ApplicationException($"This Canvas is locked by {lockedByUser}.");
            }

            segmentIndex = fileText.ToString().IndexOf(segmantName);

            readToIndex = (readToIndex - segmentIndex + 1);

            fileText.Remove(segmentIndex, readToIndex);
          }

          File.WriteAllText(this.LockFullFileName, fileText.ToString());
        }
      }
      finally
      {
        File.Delete(this.FileLockName);
      }
    }

    public void RemoveUserLocks(string userName)
    {
      if (!General.ProjectModel.LockCanvasOnEditing)
      {
        return;
      }

      if (!File.Exists(this.LockFullFileName))
      {
        return;
      }

      try
      {
        this.WaitLockRelease();

        lock (lockMethodObject)
        {
          StringBuilder fileText = new StringBuilder();

          fileText.Append(File.ReadAllText(this.LockFullFileName));

          string fileUserName = $":{userName};";

          int usernameInstanceIndex = fileText.ToString().IndexOf(fileUserName);

          while (usernameInstanceIndex > 0)
          {
            int controlIndex = usernameInstanceIndex - 1;

            while (fileText.ToString().Substring(controlIndex, 1) != ";")
            {
              --controlIndex;

              if (controlIndex < 0)
              {
                break;
              }
            }

            ++controlIndex;

            int lengthIndex = ((usernameInstanceIndex - controlIndex) + (userName.Length + 2));

            //string toRemove = fileText.ToString().Substring(controlIndex, lengthIndex);

            fileText.Remove(controlIndex, lengthIndex);

            usernameInstanceIndex = fileText.ToString().IndexOf(fileUserName);
          }

          File.WriteAllText(this.LockFullFileName, fileText.ToString());
        }
      }
      finally
      {
        File.Delete(this.FileLockName);
      }
    }

    private void WaitLockRelease()
    {
      while(File.Exists(this.FileLockName))
      {
        Sleep.ThreadWait(100);
      }

      File.WriteAllText(this.FileLockName, $"File locked by {Environment.UserName}");
    }

    private string LockFullFileName
    {
      get
      {
        return Path.Combine(General.ProjectModel.FileDirectory,  $"{General.ProjectModel.ModelName}.FileLocks.{FileTypes.eloc}");
      }
    }
  
    private string FileLockName
    {
      get
      {
        return Path.Combine(General.ProjectModel.FileDirectory, $"{General.ProjectModel.ModelName}.FileLocked");
      }
    }
  }
}
 