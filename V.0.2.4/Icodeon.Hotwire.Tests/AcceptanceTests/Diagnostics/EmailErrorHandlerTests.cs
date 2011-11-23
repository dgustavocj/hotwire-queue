using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Diagnostics
{
    // Copy this test to your own project and ensure that you change the values of sslEmailErrorHandler shown below in your app.config
    // but don't change them in the open source project and accidentally check them back in to Google code, make sure you place a 
    // copy of the test in your own test project / solution. 
    //   ;-p

    /*
        <sslEmailErrorHandler 
      subjectLinePrefix="xxx subject line prefix" 
      toAddresses="toTest1@test.com,toTest2@test.com"
      fromAddress="test@test.com" 
      fromPassword="p@ssw0rd" 
      smtpHost="smpt@mail.com" 
      smtpPort="999"/>

     */

    //[TestFixture(Category = "acceptanceTest")]
    public class EmailErrorHandlerTests : UnitTest
    {
        //[Test]
        public void ShouldSendAnEmailWhenAnExceptionOccurs()
        {
            TraceTitle("ShouldSendAnEmailWhenAnExceptionOccurs() - should send an email when an exception occurs");

            Trace("Given a module that will respond with an httpException");
            var module = new MockModule();
            var configuration = ModuleConfigurationDTOFactory.GivenModuleConfigurationForMockModule();
            var streamingContext = new MockStreamingContext("http://localhost/throw/httpexception", configuration);

            Trace("And an email LoggingError handler to handle the exceptions");
            var errorHandler = new SSLEmailErrorHandler();
            module.ProcessRequestException += errorHandler.HandleException;

            Trace("When I call BeginRequest");
            module.BeginRequest(streamingContext);

            Trace("then an email should be created in the log file.");
            // implement your own check here...
        }

    }
}
