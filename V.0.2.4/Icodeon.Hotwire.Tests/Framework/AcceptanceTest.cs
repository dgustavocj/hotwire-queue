using System;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Tests.Framework
{
    public class AcceptanceTest 
    {
        public HotwireFilesProvider FilesProvider { get; private set; }
        public IConsoleWriter ConsoleWriter { get; private set; }
        public HotLogger Logger { get; private set; }

        public AcceptanceTest()
        {
            Logger = HotLogger.GetLogger(LogFiles.AcceptanceTests);
            Logger.Trace("AcceptanceTest constructor. (base class for most acceptance tests)");
            if (!DeploymentEnvironment.IsDEBUG) throw new Exception("RELEASE mode testing currently not supported."); // ADH : Need a reason why here otherwise it just becomes legend. Possibly because of release web.config settings, need to check!
            FilesProvider = HotwireFilesProvider.GetFilesProviderInstance(HotLogger.NullLogger);
            ConsoleWriter = new ConsoleWriter();
        }

    }
}
