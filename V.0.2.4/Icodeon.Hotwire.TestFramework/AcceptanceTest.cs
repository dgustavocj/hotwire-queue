using System;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.TestFramework
{

    public class AcceptanceTest : UnitTest
    {

        public IConsoleWriter ConsoleWriter { get; private set; }

        public AcceptanceTest() 
        {
            ConsoleWriter = new ConsoleWriter();
        }

    }
}
