using ERD.Viewer.Models.CanvasModels;
using ERD.Viewer.Models.Common;
using ERD.Viewer.Shared.Enums;
using ERD.Viewer.Shared.FileOptions;
using Newtonsoft.Json;

namespace ERD.Viewer.FileManagement
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
        public delegate void FileLockChangedEvent(object sender, string fullPathName, FileSystemEventArgs e);

        public delegate void FileChangedEvent(object sender, string fullPathName, ErdCanvasModel changedModel);

        public event FileChangedEvent FileChanged;

        public event FileLockChangedEvent FileLockChanged;

        private FileSystemWatcher changeWatcher;

        private FileSystemWatcher lockWatcher;
        
        ~CanvasLocksListener()
        {
            Dispose();
        }

        public void StartWatcher()
        {
            StartFileChangeWatcher();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void File_Changed(object sender, FileSystemEventArgs e)
        {
            lock (CanvasLocks.Instance.IgnoreFiles)
            {
                if (!General.ProjectModel.LockCanvasOnEditing || CanvasLocks.Instance.IgnoreFiles.Contains(e.FullPath))
                {
                    return;
                }

                CanvasLocks.Instance.IgnoreFiles.Add(e.FullPath);
            }

            if (e.FullPath.EndsWith($".{FileTypes.eclu}"))
            {
                string[] fileData = ReadFileLines(e.FullPath);

                ErdCanvasModel segment = JsonConvert.DeserializeObject(fileData[0], typeof(ErdCanvasModel)) as ErdCanvasModel;

                FileChanged?.Invoke(sender, e.FullPath, segment);
            }
            else if (e.FullPath.EndsWith($".{FileTypes.eloc}"))
            {
                FileLockChanged(sender, e.FullPath, e);
            }
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (changeWatcher != null)
                    {
                        changeWatcher.Created -= File_Changed;

                        changeWatcher.Changed -= File_Changed;

                        changeWatcher.Deleted -= File_Changed;

                        changeWatcher.Dispose();

                        changeWatcher = null;
                    }

                    if (lockWatcher != null)
                    {
                        lockWatcher.Created -= File_Changed;

                        lockWatcher.Changed -= File_Changed;

                        lockWatcher.Deleted -= File_Changed;

                        lockWatcher.Dispose();

                        lockWatcher = null;
                    }
                }
            }
            catch
            {
                // DO NOTHING
            }
        }
    
        private void StartFileChangeWatcher()
        {
            if (changeWatcher != null && !General.ProjectModel.LockCanvasOnEditing)
            {
                Dispose();

                return;
            }

            if (changeWatcher != null || !General.ProjectModel.LockCanvasOnEditing)
            {
                return;
            }

            string fileNametoWatch = $"{General.ProjectModel.ModelName}.*";

            changeWatcher = new FileSystemWatcher(General.ProjectModel.FileDirectory);

            changeWatcher.NotifyFilter =   NotifyFilters.LastWrite
                                              | NotifyFilters.CreationTime;

            changeWatcher.Created += File_Changed;

            changeWatcher.Changed += File_Changed;

            changeWatcher.Deleted += File_Changed;

            changeWatcher.Filter = fileNametoWatch;

            changeWatcher.EnableRaisingEvents = true;
        }
    
        private string[] ReadFileLines(string fullPath)
        {
            REREAD:

            try
            {
                Paths.WaitFileRelease(fullPath);

                return File.ReadAllLines(fullPath);
            }
            catch (IOException err)
            {
                goto REREAD;
            }
        }
    }
}
