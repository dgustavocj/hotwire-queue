using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public interface IFolderWatcherScript
    {
        string ScriptName { get; }
        bool isRunning { get; }
        void Run(LoggerBase logger, IConsoleWriter console, string folderPath);
    }
}
