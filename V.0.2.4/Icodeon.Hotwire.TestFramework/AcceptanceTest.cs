using System;
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
            // ADH: 23.11.2011 wrapping fixture class constructor code in try catch because test runners (Team City, NUnit) can't report on errors in the fixture setups
            try
            {
                _logger.Trace("cxtor AcceptanceTest()");
                ConsoleWriter = new ConsoleWriter();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                throw;
            }
            
        }

    }
}
