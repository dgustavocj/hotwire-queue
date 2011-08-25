using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.DTOs;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using NUnit.Framework;

namespace Icodeon.OUIntegration.Tests.AcceptanceTests.End2EndDeploys
{
    [TestFixture]
    public class MacSignSecurityTests : UnitTest
    {

        // will implement something like this later...

        //[Test]
        //public void UsersShouldOnlyBeAbleToAccessAuthorisedUris()
        //{
        //    // given the following keys are registered
        //    // keep UserId,     PrivateKey, UriStartsWith
        //    //      Alan,       1234,       animals/cats/*
        //    //      Fred,       5678,       animals/dogs/*

        //    // then the following should be the result of requests

        //    //User     Uri                        signed      Result
        //    //Alan,    animals/cats/cat1.json,    yes,         200
        //    //Alan,    animals/cats/cat1.json,    no,          401
        //    //Alan,    animals/cats/cat1.json,    no,          401
        //}



        [Test]
        public void HackerShouldNotBeAbleToReplayRequests()
        {
            // idea -> keep a persistant record of past requests [ request log ] ( slow but 100% effective and extremely simple (avoids nonce nonsense! ;-p  ) plus makes API more usable with subsequent requests returning current status...
            // not suitable for highly transactional systems, as this will progressively slow down as reads get longer if storage is file based
            // will not be able to delete request log without resetting the private keys otherwise any request can be replayed.
            // temporary solution would be to place log in an indexed database
            // correlationId (salt) to include GMT dayOfYear + hour ( and allow 1 hour each way leeway in case of roll over ) i.e. signed at 13:59:59 and server receives it at 14:00:01 
            // then can delete all logs as soon as they are older than 3(?) hours

            throw new NotImplementedException();

        }


        [Test]
        public void
            RequestShouldBeIdempotentSoAccidentalRequestTwiceByUserOrReplayAttackByHackerShouldNotCauseAnySideEffects()
        {
            // this is more about the implementation of the request...
            // e.g. go live ... request at 10:10 am should accept the request to go live
            //                  if a subsequent request arrives (with previously used "correlationID" )10 seconds later, while still busy going live then should return "busy"
            //                  if another request arrives, then should return "ok"
            //                  if a go live 
            throw new NotImplementedException();
        }

        [Test]
        public void ShouldRespondWith200IfEndPointSecurityIsMacSignedAndRequestIsMacSignedUsingConfiguredPrivateKey()
        {
            TraceTitle("Should respond with 200 if endpoint security is mac signed and request IS mac signed using configured private key.");

            Trace("Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey");

            Trace("When a signed request is made using the same private key");

            Trace("Then the response should be 200");

        }


        [Test]
        public void ShouldRespondWith401UnauthorizedIfEndPointSecurityIsMacSignedAndRequestisNotMacSignedUsingConfiguredPrivateKey()
        {
            TraceTitle("Should respond with 401 Unauthorized if endpoint security is mac signed and request is not mac signed using configured private key.");

            Trace("Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey");

            string privateKey = "4497BB2A-6782-4403-970C-1A7F50BBC7CB";
            var module = new MockModule();
            var configuration = ModuleConfigurationDTOFactory.GivenModuleConfiguration("test");
            var endpoint = new EndpointConfiguration
                               {
                                   Action = MockModule.ActionEcho,
                                   Active = true,
                                   HttpMethods = new[] {"GET"},
                                   MediaType = eMediaType.text,
                                   Name = "endpoint1",
                                   UriTemplateString = "/echo/{SAY}",
                                   Security = SecurityType.simpleMAC,
                                   PrivateKey = privateKey
                               };
            configuration.AddEndpoint(endpoint);

            Trace("When an unsigned request is made using a different private key");
            var context = new MockStreamingContext("http://localhost/test/echo/helloWorld", configuration);
            module.BeginRequest(context);

            Trace("Then the response should be 401");
            context.HttpWriter.ShouldBe401UnAuthorised();


            //Trace("When an signed request is made using a different private key");
            //var context = new MockStreamingContext("http://localhost/echo/hello+world", configuration);

            //Trace("Then the response should be 401");


            //Trace("When an signed request is made using the correct private key");
            //var context = new MockStreamingContext("http://localhost/echo/hello+world", configuration);

            //Trace("Then the response should be 200");

        }


        [Test]
        public void ShouldRespondWith500AndHumanReadableTextIfSaltIsNotAGuid()
        {
            throw new NotImplementedException();

        }


    }
}
