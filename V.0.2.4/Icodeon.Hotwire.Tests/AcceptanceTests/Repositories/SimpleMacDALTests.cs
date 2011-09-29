using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.DAL.Params;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.DAL;
//using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Repositories
{
    [TestFixture]
    public class SimpleMacDALTests : UnitTest
    {
        
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            // teardown the database 
            using(var db = new HotwireContext(ConnectionStringManager.HotwireConnectionString))
            {
                if (db.DatabaseExists()) db.DeleteDatabase();
                db.CreateDatabase();
            }
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            // dont delete the database, in cae we need to manually inspect it
            //using (var db = new HotwireContext(ConnectionStringManager.HotwireUnitTestConnectionString))
            //{
            //    if (db.DatabaseExists()) db.DeleteDatabase();
            //}
        }

        private HotwireContext _context;

        //[SetUp]
        //public void Setup()
        //{
         
        //}


        [Test]
        public void ShouldBeAbleToRecordRequest()
        {
            TraceTitle("Should be able to record requests");

            using (var context = new HotwireContext(ConnectionStringManager.HotwireConnectionString))
            {
                Trace("Given a simple SimpleMac DAL");
                var dal = new SimpleMacDal(new DateTimeWrapper(), context);

                Trace("Given no requests have been made");
                context.SimpleMacHistories.Count().Should().Be(0);

                Trace("When I cache a new request");

                var startTime = DateTime.Now;
                var salt = Guid.NewGuid();
                const string url = "http://api.somedomain.com/test/a/b/c?parameter=123";
                const int ms = 10 * 1000;
                dal.CacheRequest(new CacheRequestParams(salt, url, ms));
                context.SubmitChanges();

                Trace("then the request should be recorded");
                var msh = context.SimpleMacHistories.FirstOrDefault();

                msh.Url.Should().Be(url); // todo : test that should truncate at 200
                msh.Salt.Should().Be(salt);
                msh.Expires.Should().BeAfter(startTime.AddMilliseconds(ms - 1));
            }

        }

        [Test]
        public void ShouldDetermineIfRequestAlreadyExists()
        {
            TraceTitle("Should determine if request already exists");

            using (var context = new HotwireContext(ConnectionStringManager.HotwireConnectionString))
            {
                Trace("Given a request is made");
                var dal = new SimpleMacDal(new DateTimeWrapper(), context);

                var request = createRequest();
                dal.CacheRequest(request);                
               context.SubmitChanges();

                Trace("when I check if the request has been made");
                bool wasmade = dal.RequestsExists(request.Salt);

                Trace("then the result should be true");
                wasmade.Should().BeTrue();

                Trace("Given a request has not been made");
                var request2 = new CacheRequestParams(Guid.NewGuid(), "dsfsfsf", 10000);

                Trace("when I check if the request has been made");
                wasmade = dal.RequestsExists(request2.Salt);
                
                Trace("Then the result should be false");
                wasmade.Should().BeFalse();
            }
        }


        private CacheRequestParams createRequest()
        {
            var salt = Guid.NewGuid();
            const string url = "http://api.somedomain.com/test/a/b/c?parameter=123";
            const int ms = 10 * 1000;
            return new CacheRequestParams(salt,url,ms);
        }

        //[Test]
        //public void ShouldBeAbleToRemoveExpiredItems()
        //{
        //    //throw new NotImplementedException();
        //}

    }
}
