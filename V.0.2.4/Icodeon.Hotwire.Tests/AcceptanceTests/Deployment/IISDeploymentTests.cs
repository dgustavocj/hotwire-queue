using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Deployment;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using Microsoft.Web.Administration;
using NLog;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Deployment
{
    [TestFixture]
    [Ignore("Not needed yet")]
    public class IISDeploymentTests : UnitTest
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        readonly string _testDomainName = "test.IISDeploymentTests";
        private readonly string _defaultWebsiteMarkerText = "34BC28B7-CA23-491F-A1CA-060CEFB53B44";
        int _port = int.Parse(ConfigurationManager.AppSettings["Icodeon.Hotwire.Tests.AcceptanceTests.Deployment.IISDeploymentTests-Port"]);
        private string _rootFolder;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _logger.LoggedExecution(() =>{
                _logger.Debug("IISDeploymentTests.FixtureSetup()");
                _rootFolder = Path.Combine(Environment.CurrentDirectory, @"App_Data\golive");
                var dm = new DeployManager();
                dm.DeleteAllSitesStartingWith(_testDomainName);
                dm.DeleteApplicationPoolIfExists(_testDomainName);
                dm.CreateApplicationPoolIfNotExist(_testDomainName);
            });
        }


        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            _logger.LoggedExecution(() =>{
                _logger.Debug("IISDeploymentTests.FixtureTearDown()");
                var dm = new DeployManager();
                dm.DeleteAllSitesStartingWith(_testDomainName);
                dm.DeleteApplicationPoolIfExists(_testDomainName);
            });
        }


        // security assert ? (we need to be running as administrator, otherwise we can't create host entry)
        [Test]
        [Ignore("not needed yet. Will fix later")]
        public void ShouldCreateAndTakeDownIISWebsites()
        {
            
            TraceTitle("end to  end happy case");

            using (var hosts = new LMHosts(createBackup:true))
            {

                Trace("given the LHMOSTS file contains entry 'test.IISDeploymentTests' ");
                Trace("and three folders golive/v1 /golive/v2 and golive/v3");
                Trace("with 3 versions of the same website, that return 'hello world 1!' 'Hello world 2!' and 'Hello world 3!' in each folder respetively");
                //--------------------------------------------------------------------------------------
                var dm = new DeployManager();
                hosts.AddHostEntryIfNotExist(_testDomainName, "localhost");

                // When I call GoLive versions 1,2,3,2,1 respectively
                // Then the appropriate version should be live each time
                //--------------------------------------------------------------------------------------
                var vm = new VersionedSiteManager(_rootFolder, _testDomainName, _testDomainName, _port);
                var versions = new List<string> {"1", "1", "3", "1","2", "3", "2","1"}; 
                string result = null;
                versions.ForEach(v =>{
                    Trace("when I call goLive('{0}')", v);
                    vm.GoLive(v);
                    string url = string.Format("http://{0}:{1}", _testDomainName, _port);
                    Trace("And make a request '{0}'", url);
                    result = new WebClient().DownloadString(url);
                    string expected = "hello world " + v + "!";
                    Trace("Then the result should contain '" + expected + "'");
                    result.Should().Contain(expected);
                });
            }
            // just to prove that our hosts file is as it was before the test started
            new LMHosts(createBackup: false).Lines.Should().NotContain(_testDomainName);
        }


    }
}
