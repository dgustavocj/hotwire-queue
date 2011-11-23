using System;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.TestFramework
{

    public class AcceptanceTest : UnitTest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public IConsoleWriter ConsoleWriter { get; private set; }

        public AcceptanceTest()
        {
            _logger.LoggedExecution("cxtor AcceptanceTest()", () =>
            {
                ConsoleWriter = new ConsoleWriter();
            });
        }

    }
}
