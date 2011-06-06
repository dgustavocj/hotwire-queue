using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class ProviderFactoryTests : UnitTest
    {

        public class MyClassFactoryTest : IClassFactoryTestImplemented
        {
            public string Message()
            {
                return "MyClassFactoryTest";
            }
        }

        // it can't because if "I" implement something, then it doesnt know which one to return since there are two. 

        [Test]
        public void ShouldReturnMyImplementationIfInterfaceIsImplemented()
        {
            Trace("given I have implemented one of the autowired up interfaces");
            // MyClassFactoryTest implements IClassFactoryTestImplemented
            
            Trace("when I call autoWireUp and request an instance");
            var providerFactory = new ProviderFactory();
            providerFactory.AutoWireUpProviders();
            var testClass = providerFactory.GetProvider<IClassFactoryTestImplemented>();

            Trace("then the factory should return my implemented class");
            testClass.Should().NotBeNull();
            testClass.Should().BeOfType<MyClassFactoryTest>();
            testClass.Message().Should().Be("MyClassFactoryTest");
        }



        [Test]
        public void ShouldReturnTheDefaultIfNoClassImplemented()
        {
            Trace("given I have not implemented one of the auto wired up interfaces");
            Trace("when I call autoWireUp");
            var providerFactory = new ProviderFactory();
            providerFactory.AutoWireUpProviders();
            Trace("and request an instance");
            var testClass = providerFactory.GetProvider<IClassFactoryNotImplemented>();

            Trace("then the factory should return the framework's default");
            testClass.Should().NotBeNull();
            testClass.Should().BeOfType<DefaultForClassFactoryNotImplemented>();
            testClass.Greet().Should().Be("I am a DefaultForClassFactoryNotImplemented");
        }


    }


}
