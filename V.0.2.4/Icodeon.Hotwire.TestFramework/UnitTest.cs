using System;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Diagnostics;
using NLog;

namespace Icodeon.Hotwire.TestFramework
{
    public class UnitTest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public HotLogger Logger { get; private set; }

        public UnitTest()
        {
            _logger.LoggedExecution("cxtor UnitTest()",() =>{
                Logger = HotLogger.GetCurrentClassLogger();
                Logger.EchoToConsole = true;
            });
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
