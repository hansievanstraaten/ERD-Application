using System.Threading.Tasks;
using System.Configuration;
using System;
using System.IO;
using ERD.Common;
using ERD.Base;
using ERD.Models;
using ViSo.Common;
using Newtonsoft.Json;

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
        public delegate void FileLockChangedEvent(object sender, string fullPathName, FileSystemEventArgs e);

        public delegate void FileChangedEvent(object sender, string fullPathName, ErdCanvasModel changedModel);

        public event FileChangedEvent FileChanged;

        public event FileLockChangedEvent FileLockChanged;

        private FileSystemWatcher changeWatcher;

        private FileSystemWatcher lockWatcher;
        
        ~CanvasLocksListener()
        {
            this.Dispose();
        }

        public void StartWatcher()
        {
            this.StartFileChangeWatcher();
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
                string[] fileData = this.ReadFileLines(e.FullPath);

                ErdCanvasModel segment = JsonConvert.DeserializeObject(fileData[0], typeof(ErdCanvasModel)) as ErdCanvasModel;
                
                this.FileChanged?.Invoke(sender, e.FullPath, segment);
            }
            else if (e.FullPath.EndsWith($".{FileTypes.eloc}"))
            {
                this.FileLockChanged(sender, e.FullPath, e);
            }
        }

        private void Dispose(Boolean disposing)
        {
            try
            {
                if (disposing)
                {
                    if (this.changeWatcher != null)
                    {
                        this.changeWatcher.Created -= File_Changed;

                        this.changeWatcher.Changed -= File_Changed;

                        this.changeWatcher.Deleted -= File_Changed;

                        this.changeWatcher.Dispose();

                        this.changeWatcher = null;
                    }

                    if (this.lockWatcher != null)
                    {
                        this.lockWatcher.Created -= File_Changed;

                        this.lockWatcher.Changed -= File_Changed;

                        this.lockWatcher.Deleted -= File_Changed;

                        this.lockWatcher.Dispose();

                        this.lockWatcher = null;
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
            if (this.changeWatcher != null && !General.ProjectModel.LockCanvasOnEditing)
            {
                Dispose();

                return;
            }

            if (this.changeWatcher != null || !General.ProjectModel.LockCanvasOnEditing)
            {
                return;
            }

            string fileNametoWatch = $"{General.ProjectModel.ModelName}.*";

            this.changeWatcher = new FileSystemWatcher(General.ProjectModel.FileDirectory);

            this.changeWatcher.NotifyFilter =   NotifyFilters.LastWrite
                                              | NotifyFilters.CreationTime;

            this.changeWatcher.Created += this.File_Changed;

            this.changeWatcher.Changed += this.File_Changed;

            this.changeWatcher.Deleted += this.File_Changed;

            this.changeWatcher.Filter = fileNametoWatch;

            this.changeWatcher.EnableRaisingEvents = true;
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
