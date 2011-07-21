using Icodeon.Hotwire.Framework;

namespace Icodeon.Hotwire.TestFramework
{
    public class UnitTest
    {
        public HotLogger Logger { get; private set; }

        public UnitTest()
        {
            Logger = HotLogger.GetCurrentClassLogger();
            Logger.EchoToConsole = true;
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
