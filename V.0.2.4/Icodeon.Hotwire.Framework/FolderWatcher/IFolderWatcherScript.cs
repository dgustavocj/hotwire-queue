using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public interface IFolderWatcherScript
    {
        string ScriptName { get; }
        bool isRunning { get; }
        void Run(IConsoleWriter console, string folderPath);
    }
}
