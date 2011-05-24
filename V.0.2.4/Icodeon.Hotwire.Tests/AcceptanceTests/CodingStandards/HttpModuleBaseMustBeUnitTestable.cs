using System;
using System.Collections.Generic;
using System.Threading;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Tests.Framework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.CodingStandards
{
    [TestFixture]
    public class HttpModuleBaseMustBeUnitTestable : UnitTest
    {
        [Test]
        public void HttpRequestsWithUrlsMatchingEndpointConfigMustTriggerTheModule()
        {
            Logger.TraceTitle("EndpointConfigurationMustBeUnitTestable");
            //===========================================================

            Logger.Trace("given an httpModule");
            Logger.Trace("and an endpoint configuration");
            //============================================
            //var http = new TestHttpModule();

            Logger.Trace("when I trigger the endpoint");
            //==========================================

            Logger.Trace("then the configured action for the matching endpoint is passed to the module as a parameter");
            //==========================================================================================================

            Assert.Inconclusive("Not yet implemented.");

        }

        //public class TestHttpModule :  ModuleBase
        //{
        //    protected override string ConfigurationSectionName
        //    {
        //        get { throw new NotImplementedException(); }
        //    }

        //    public override IEnumerable<string> ActionNames
        //    {
        //        get { yield return "test"; }
        //    }

        //    protected override object ProcessRequest(System.Web.HttpApplicationState applicationState, System.IO.Stream inputStream, Uri url, UriTemplateMatch match, Hotwire.Framework.Configuration.IModuleEndpoint config, Hotwire.Framework.Contracts.IMediaInfo mediaInfo, Hotwire.Framework.Utils.IMapPath mapper, Hotwire.Framework.Diagnostics.LoggerBase logger)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
