using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Tests.Framework
{
    public class UnitTest
    {
        public HotLogger Logger { get; private set; }

        public UnitTest(string logger)
        {
            Logger = HotLogger.GetLogger(logger);
            Logger.Trace("UnitTest constructor. (base class for most unit tests.)");
        }


        public UnitTest()
        {
            Logger = HotLogger.GetLogger(LogFiles.UnitTests);
            Logger.Trace("UnitTest constructor. (base class for most unit tests.)");
        }

        public void Trace(string message, params object[] parameters)
        {
            Logger.Trace(message, parameters);
        }

        public void TraceTitle(string testTitle, params object[] parameters)
        {
            Logger.TraceTitle(testTitle, parameters);
        }

    }
}
