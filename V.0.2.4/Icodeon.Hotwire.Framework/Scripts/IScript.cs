using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Scripts
{
    public interface IScript
    {
        bool isRunning { get; }
        string ScriptName { get; }
        void Run(IConsoleWriter console);
    }
}
