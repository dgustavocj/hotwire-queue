using System;
using Icodeon.Hotwire.Framework;
using NLog;

namespace Icodeon.Hotwire.TestFramework
{
    public class UnitTest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public HotLogger Logger { get; private set; }

        public UnitTest()
        {
            _logger.Trace("cxtor UnitTest()");
            try
            {
                Logger = HotLogger.GetCurrentClassLogger();
                Logger.EchoToConsole = true;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                throw;
            }
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
