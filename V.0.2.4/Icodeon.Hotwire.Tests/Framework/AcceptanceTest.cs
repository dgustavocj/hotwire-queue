using System;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Tests.Framework
{

    /// <summary>
    /// AcceptanceTest class uses filesProvider and therefore requires that you completely define all the hotwire folders in your app.config, otherwise you will get null reference.
    /// </summary>
    public class AcceptanceTest 
    {
        public HotwireFilesProvider FilesProvider { get; private set; }
        public IConsoleWriter ConsoleWriter { get; private set; }
        public HotLogger Logger { get; private set; }

        public AcceptanceTest()
        {
            Logger = HotLogger.GetLogger(LogFiles.AcceptanceTests);
            Logger.Trace("AcceptanceTest constructor. (base class for most acceptance tests)");
            // CI?
            // TODO: need to include test to see what server this code has been deployed on! We will assume for now that 
            if (!DeploymentEnvironment.IsDEBUG) throw new Exception("RELEASE mode testing currently not supported on dev machines. If these tests will run on CI server then this test needs to be reversed!"); 
            
            FilesProvider = HotwireFilesProvider.GetFilesProviderInstance(HotLogger.NullLogger);
            ConsoleWriter = new ConsoleWriter();
        }

    }
}
