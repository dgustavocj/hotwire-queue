using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public interface IFolderWatcherScript
    {
        string ScriptName { get; }
        bool isRunning { get; }
        void Run(Logger logger, IConsoleWriter console, string folderPath);
    }
}
