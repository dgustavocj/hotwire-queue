using System;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.TestFramework
{

    public class AcceptanceTest : UnitTest
    {

        public IConsoleWriter ConsoleWriter { get; private set; }

        public AcceptanceTest() 
        {
            // CI?
            // TODO: need to include test to see what server this code has been deployed on! We will assume for now that 
            if (!DeploymentEnvironment.IsDEBUG) throw new Exception("RELEASE mode testing currently not supported on dev machines. If these tests will run on CI server then this test needs to be reversed!"); 
            
            ConsoleWriter = new ConsoleWriter();
        }

    }
}
