using System.IO;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public class FolderScript
    {
        public FolderConfig Config { get; set; }
        public IFolderWatcherScript Script { get; set; }
        public FileSystemWatcher Watcher { get; set; }
    }
}