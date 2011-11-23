using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;
using StructureMap;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    // class that proves that structuremap works the way we need it to work, or that we think it is working for our purposes.

    [TestFixture]
    public class HowWeAreUsingStructureMapTests : UnitTest
    {
        [Test]
        public void ProveThatRequestingWiredUpObjectForInterfaceThatIsNotYetImplementedShouldThrowException()
        {
            TraceTitle("ProveThatRequestingWiredUpObjectForInterfaceThatIsNotYetImplementedShouldThrowException()");
            INoOneHasImplementedThisUnlessTheyAreAnIdiotAndWantOurTestToFail expectedObject = null;
            Action action = ()=> expectedObject = ObjectFactory.GetInstance<INoOneHasImplementedThisUnlessTheyAreAnIdiotAndWantOurTestToFail>();
            action.ShouldThrow<StructureMapException>();
        }

        [Test]
        public void ProveWeCANNOTInitializeObjectfactoryMoreThanOnceWithoutWipingOutPreviousConfiguration()
        {
            TraceTitle("ProveWeCANNOTInitializeObjectfactoryMoreThanOnceWithoutWipingOutPreviousConfiguration() - Prove we CANNOT initialize objectfactory more than once without wiping out previous configurations!");
            
            Trace("Wire up a talker");
            ObjectFactory.Initialize(x => x.For<ITalk>().Use<Talker>());

            Trace("Check getting a talker works");
            var person1 = ObjectFactory.GetInstance<ITalk>();
            person1.Talk().Should().Be("I am a talker");

            Trace("Now wire up a runner using initialise");
            ObjectFactory.Initialize(x => x.For<IRun>().Use<Runner>());
            var person2 = ObjectFactory.GetInstance<IRun>();
            Trace("and make sure we get a proper runner back");
            person2.Should().NotBeNull();
            person2.WhatAreYouDoing().Should().Be("running!");

            Trace("However, since we called initialise, trying to get a talker again should throw StructureMapException because it's been wiped!");
            ITalk person3 = null;
            Action action = ()=> person3 = ObjectFactory.GetInstance<ITalk>();
            action.ShouldThrow<StructureMapException>();
        }

        [Test]
        public void ProveWeCanAddTotheConfigurationMoreThanOnceWithoutWipingOutPreviousConfiguration()
        {
            TraceTitle("ProveWeCanAddTotheConfigurationMoreThanOnceWithoutWipingOutPreviousConfiguration() - Prove we can add to the configuration more than once without wiping out previous configuration provided we use configure, not initialize.");

            Trace("Wire up a talker");
            ObjectFactory.Initialize(x => x.For<ITalk>().Use<Talker>());

            Trace("Check getting a talker works");
            var person1 = ObjectFactory.GetInstance<ITalk>();
            person1.Talk().Should().Be("I am a talker");

            Trace("Now wire up an additional runner using configure");
            ObjectFactory.Configure(x => x.For<IRun>().Use<Runner>());
            var person2 = ObjectFactory.GetInstance<IRun>();
            Trace("and make sure we get a proper runner back");
            person2.Should().NotBeNull();
            person2.WhatAreYouDoing().Should().Be("running!");

            Trace("Since we called configure, trying to get a talker again should NOT throw StructureMapException because it should not have been wiped!");
            ITalk person3 = null;
            Action action = () => person3 = ObjectFactory.GetInstance<ITalk>();
            action.ShouldNotThrow();
            person3.Should().NotBeNull();
            person3.Talk().Should().Be("I am a talker");
        }


    }


    public class Runner : IRun
    {
        public string WhatAreYouDoing()
        {
            return "running!"; 
        }
    }

    public class Screamer : ITalk
    {
        public string Talk() { return "aaai!"; }
    }

    public class Talker : ITalk
    {
        public string Talk() { return "I am a talker"; }
    }

    public interface IRun
    {
        string WhatAreYouDoing();
    }

    public interface ITalk
    {
        string Talk();
    }

    public interface INoOneHasImplementedThisUnlessTheyAreAnIdiotAndWantOurTestToFail
    {
        
    }
}
