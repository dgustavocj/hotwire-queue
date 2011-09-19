using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Repositories;
//using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Repositories
{
    [TestFixture]
    public class MacSaltCacheTests : UnitTest
    {
        private DirectoryInfo _appDataFolder = new DirectoryInfo(DirectoryHelper.MapProjectSubfolder("App_Data", HotwireFilesProvider.MarkerFiles.AppDataFolder));

       // private SimpleMacModel _db = new SimpleMacModel(MacSaltDAL.ConnectionString);
        
        [TestFixtureSetUp]
        void FixtureSetup()
        {
            var cache = new SimpleMacDal(SimpleMacDal.ConnectionString);
            if (_appDataFolder.GetFiles("*.mdf").Count()!=0) cache.Repository.Delete();
            cache.Repository.Create();
        }

        [TestFixtureTearDown]
        void FixtureTearDown()
        {
            if (_appDataFolder.Exists) new SimpleMacDal(SimpleMacDal.ConnectionString).Repository.Delete();
        }


        [SetUp]
        void Setup()
        {

        }

        // unfortunately not sure if we can test this practically as we're already creating and delete the db during fixture setup and teardown
        // which will have to do for our tests.

        //[Test]
        //public void ShouldBeAbleToCreateAndDeleteTheRepository()
        //{
        //    TraceTitle("Should be able to create the repository");
        //    Trace("Given no database in the app_data folder");
        //    _appDataFolder.GetFiles("*.mdf").Count().Should().Be(0);

        //    Trace("When I create the repository");
        //    string cs = MacSaltCache.ConnectionString;
        //    var cache = new MacSaltCache(cs);
        //    cache.Repository.Create();

        //    Trace("Then the repository database should exist");
        //    _appDataFolder.GetFiles("*.mdf").Count().Should().Be(1);

        //    Trace("");
        //    TraceTitle("Should be able to Delete the repository");
        //    Trace("Given a database exists in the app_data folder");
        //    _appDataFolder.GetFiles("*.mdf").Count().Should().Be(1);

        //    Trace("When I delete the repository");
        //    cache.Repository.Delete();

        //    Trace("Then the repository database should no longer exist");
        //    _appDataFolder.GetFiles("*.mdf").Count().Should().Be(0);

        //}

        // uncomment code below and manually run the unit test to delete any database lying around...
        // NB! DO NOT DELETE DB FILES MANUALLY! Otherwise sql express will contain references to db files that dont exist, very bad!

        //[Test]
        //public void ShouldBeAbleToDeleteDatabase()
        //{
        //    string cs = MacSaltCache.ConnectionString;
        //    var cache = new MacSaltCache(cs);

        //    TraceTitle("Should be able to Delete the repository");
        //    Trace("Given a database exists in the app_data folder");
        //    //_appDataFolder.GetFiles("*.mdf").Count().Should().Be(1);

        //    Trace("When I delete the repository");
        //    cache.Repository.Delete();

        //    Trace("Then the repository database should no longer exist");
        //    _appDataFolder.GetFiles("*.mdf").Count().Should().Be(0);
        //}

        [Test]
        public void ShouldBeAbleToRecordRequest()
        {
            throw new NotImplementedException();
            TraceTitle("Should be able to record requests");

            Trace("Given a macSalt cache");
            var cache = new SimpleMacDal(SimpleMacDal.ConnectionString);

            Trace("Given no requests have been made");
           // _db.MacSaltHistories.Count().Should().Be(0);

            Trace("When I cache a new request");

            DateTime startTime = DateTime.Now;
            string mac = "1234"; // TODO .. use more realistic sample value
            Guid salt = Guid.NewGuid();
            string url = "http://api.somedomain.com/test/a/b/c?parameter=123";
            int ms = 10*1000;
            cache.CacheRequest("1234", salt, url, ms);
            
            //Trace("then the request should be recorded");
            //var msh = _db.MacSaltHistories.FirstOrDefault();

            //msh.Mac.Should().Be(mac);
            //msh.Url.Should().Be(url); // todo : test that should truncate at 50?
            //msh.Salt.Should().Be(salt);
            //msh.Expires.Should().BeAfter(startTime.AddMilliseconds(ms-1));
        }

        [Test]
        public void ShouldDetermineIfRequestAlreadyExists()
        {
            //throw new NotImplementedException();
        }

        [Test]
        public void ShouldBeAbleToRemoveExpiredItems()
        {
            //throw new NotImplementedException();
        }

    }
}
