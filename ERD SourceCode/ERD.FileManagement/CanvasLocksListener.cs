using System.Threading.Tasks;
using System.Configuration;
using System;
using System.IO;
using ERD.Common;
using ERD.Base;

namespace ERD.FileManagement
{
  /// <summary>
  /// Singleton:
  /// ==========
  /// - Singleton Object for ???.
  /// - Used to ensure that only one instance of the 
  ///   of the ??? exists at any one time
  /// - Ensure that ??? exists and is properly instantiated,
  ///   if already instantiated then use the same ???.
  /// </summary>
  public class CanvasLocksListener : IDisposable
  {
    public delegate void FileChangedEvent(object sender, FileSystemEventArgs e);

    public event FileChangedEvent FileChanged;

    private FileSystemWatcher watcher;

    ~CanvasLocksListener()
    {
      this.Dispose();
    }

    public void StartWatcher()
    {
      if (this.watcher != null && !General.ProjectModel.LockCanvasOnEditing)
      {
        Dispose();

        return;
      }

      if (this.watcher != null || !General.ProjectModel.LockCanvasOnEditing)
      {
        return;
      }

      string fileNametoWatch = $"{General.ProjectModel.ModelName}.FileLocks.{FileTypes.eloc}";

      this.watcher = new FileSystemWatcher(General.ProjectModel.FileDirectory);

      this.watcher.NotifyFilter = NotifyFilters.FileName
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.Attributes
                                | NotifyFilters.Size
                                | NotifyFilters.LastWrite
                                | NotifyFilters.LastAccess
                                | NotifyFilters.CreationTime;
      //| NotifyFilters.Security;

      this.watcher.Created += File_Changed;

      this.watcher.Changed += File_Changed;

      this.watcher.Deleted += File_Changed;

      this.watcher.Filter = fileNametoWatch;

      this.watcher.EnableRaisingEvents = true;
    }

    public void Dispose()
    {
      Dispose(true);
    }

    private void File_Changed(object sender, FileSystemEventArgs e)
    {
      if (!General.ProjectModel.LockCanvasOnEditing)
      {
        return;
      }

      FileChanged?.Invoke(sender, e);
    }

    private void Dispose(Boolean disposing)
    {
      try
      {
        if (disposing)
        {
          if (this.watcher == null)
          {
            return;
          }

          this.watcher.Created -= File_Changed;

          this.watcher.Changed -= File_Changed;

          this.watcher.Deleted -= File_Changed;

          this.watcher.Dispose();

          this.watcher = null;
        }
      }
      catch
      {
        // DO NOTHING
      }
    }
  }
}
