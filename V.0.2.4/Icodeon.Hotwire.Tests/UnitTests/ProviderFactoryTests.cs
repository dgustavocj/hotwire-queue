﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class ProviderFactoryTests : UnitTest
    {
        [SetUp]
        public void SetUp()
        {
            new ProviderFactory().ClearConfiguration();
        }

        public class MyClassFactoryTest : IClassFactoryTestImplemented
        {
            public string Message()
            {
                return "MyClassFactoryTest";
            }
        }

        // need a test proving that provider factory needs a clear state?


        [Test]
        public void AttemptingToRequestInstanceFromFactoryWithoutFirstWiringUpShouldThrowExceptionEvenWithNewFactoryInstances()
        {
            // there was a problem with state remaining between tests, this is a test to PROVE that we DO
            // NEED a clearConfiguration method that test setups can call!
            TraceTitle("Attempting to request instance from factory without first wiring up should throw exception even with new factory instances.");

            Trace("given provider factory A");
            var providerFactoryA = new ProviderFactory();

            Trace("and I have implemented one of the autowired up interfaces");
            // MyClassFactoryTest implements IClassFactoryTestImplemented

            Trace("when I call autoWireUp (A)");
            providerFactoryA.AutoWireUpProviders();

            Trace("and request an instance");
            IClassFactoryTestImplemented testClass;
            Action actionA = () => testClass = providerFactoryA.GetProvider<IClassFactoryTestImplemented>();
            actionA.ShouldNotThrow();
            Trace("then the factory should not throw an exception");

            Trace("Given new provider factory B");
            var providerFactoryB = new ProviderFactory();

            Trace("And request the same instance again from provider factoryB");
            Action actionB = () => testClass = providerFactoryA.GetProvider<IClassFactoryTestImplemented>();

            Trace("then the factoryB should not throw an exception because it's also wired up.");
            actionB.ShouldNotThrow();


            Trace("when I clear the configuration in A");
            providerFactoryA.ClearConfiguration();

            Trace("and make a third request from a new provider FactoryC");
            var providerFactoryC = new ProviderFactory();
            Action actionC = () => testClass = providerFactoryC.GetProvider<IClassFactoryTestImplemented>();

            Trace("then the factoryC should throw notWiredUpException");
            actionC.ShouldThrow<NotWiredUpException>();
        }

        [Test]
        public void ShouldThrowExceptionIfNotAutomaticallyOrFluentlyWiredUpBeforeUse()
        {
            TraceTitle("Should throw exception if not automatically or fluently wired up before use");

            Trace("given a provider factory");
            var providerFactory = new ProviderFactory();

            Trace("when I request an instance without automatically wiring up");

            IClassFactoryTestImplemented myclass;
            Action action = () => myclass = providerFactory.GetProvider<IClassFactoryTestImplemented>();

            Trace("then the factory should throw not wired up exception");
            action.ShouldThrow<NotWiredUpException>();
        }

        [Test]
        public void ShouldNotThrowExceptionIfAutomaticallyWiredUpBeforeUse()
        {
            Trace("given I have implemented one of the autowired up interfaces");
            // MyClassFactoryTest implements IClassFactoryTestImplemented

            Trace("when I call autoWireUp");
            var providerFactory = new ProviderFactory();
            providerFactory.AutoWireUpProviders();

            Trace("and request an instance");
            IClassFactoryTestImplemented testClass;
            Action action = ()=> testClass = providerFactory.GetProvider<IClassFactoryTestImplemented>();
            action.ShouldNotThrow();
            Trace("then the factory should not throw an exception");
        }



        
        [Test]
        public void ClearConfigurationShouldRemoveAnyPreviousWirings()
        {
            Trace("Given a provider factory");
            var providerFactory = new ProviderFactory();

            Trace("and I have implemented one of the autowired up interfaces");
            // MyClassFactoryTest implements IClassFactoryTestImplemented


            Trace("and I fluently wire up a provider");
            providerFactory.WireUp<IClassFactoryTestImplemented, MyClassFactoryTest>();

            Trace("When I request a class from the provider");
            IClassFactoryTestImplemented myclass = null;
            Action action = () => myclass = providerFactory.GetProvider<IClassFactoryTestImplemented>();

            Trace("then the factory should not throw an exception");
            action.ShouldNotThrow();

            Trace("When I clear the factory configuration");
            providerFactory.ClearConfiguration();

            Trace("Then the wiring up should no longer be in the configuration");
            var providerFactory2 = new ProviderFactory();
            IClassFactoryTestImplemented myclass2 = null;
            Action action2 = () => myclass2 = providerFactory2.GetProvider<IClassFactoryTestImplemented>();
            action2.ShouldThrow<NotWiredUpException>();

        }

        [Test]
        public void ShouldNotThrowExceptionIfFluentlyWiredUpBeforeUse()
        {
            Trace("Given a provider factory");
            var providerFactory = new ProviderFactory();
            
            Trace("and I have implemented one of the autowired up interfaces");
            // MyClassFactoryTest implements IClassFactoryTestImplemented

            
            Trace("and I fluently wire up a provider");
            providerFactory.WireUp<IClassFactoryTestImplemented, MyClassFactoryTest>();

            Trace("When I request a class from the provider");
            IClassFactoryTestImplemented myclass =null;
            Action action = ()=> myclass = providerFactory.GetProvider<IClassFactoryTestImplemented>();

            Trace("then the factory should not throw an exception");
            action.ShouldNotThrow();
            //myclass.Should().NotBeNull();
            //myclass.Should().BeOfType<MyClassFactoryTest>();
            //myclass.Message().Should().Be("MyClassFactoryTest");

        }




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


        [Test]
        public void ShouldBeAbleOverrideAutomaticWiringUpDefaultsWithFluentWirings()
        {
            // before overriding I should get the default implementation (loggingFileProcessor)
            // ********************************************************************************
            Trace("Given an automatically wired up provider");

            var providerFactory = new ProviderFactory();
            providerFactory.AutoWireUpProviders();

            Trace("When I request instance of ProcessFile");

            IFileProcessorProvider fpp = null;
            Action action = () => fpp = providerFactory.GetProvider<IFileProcessorProvider>();

            Trace("Then I the instance returned should be the default implementation");
            action.ShouldNotThrow();
            fpp.Should().NotBeNull();
            fpp.Should().BeOfType<LoggingFileProcessorProvider>();

            // but when I configure it fluently, then this should override automatic implementation
            // ************************************************************************************
            Trace("Given an automatically wired up provider factory");
            providerFactory.ClearConfiguration();
            providerFactory.AutoWireUpProviders();

            Trace("When I fluently wire up MY implementation of IProcessFile");
            providerFactory.WireUp<IFileProcessorProvider, DummyProcessorProvider>();

            Trace("When I request instance of ProcessFile");
            Action action2 = () => fpp = providerFactory.GetProvider<IFileProcessorProvider>();

            Trace("Then the instance returned should be my implementation (i.e. fluent should override automatic.)");
            action2.ShouldNotThrow();
            fpp.Should().NotBeNull();
            fpp.Should().BeOfType<DummyProcessorProvider>();
        }

        [Test]
        public void ShouldBeAbleToFluentlyWireUpMultipleProviders()
        {
            Trace("Given two dummy classes that implement two dummy interfaces");

            Trace("and I fluently wire up a provider");
            ProviderFactory providerFactory = new ProviderFactory()
                .WireUp<IDummy1, Dummy1>()
                .WireUp<IDummy2, Dummy2>();

            Trace("When I request instances of dummy1 and dummy2");
            IDummy1 d1= null;
            IDummy2 d2 = null;
            Action action = () => {
                d1 = providerFactory.GetProvider<IDummy1>();
                d2 = providerFactory.GetProvider<IDummy2>();
            };

            Trace("then the factory should not throw an exception");
            action.ShouldNotThrow();

            d1.Should().NotBeNull();
            d1.Should().BeOfType<Dummy1>();
            d1.Name.Should().Be("Dummy1");

            d2.Should().NotBeNull();
            d2.Should().BeOfType<Dummy2>();
            d2.Size.Should().Be(42);

        }

    }



    public interface IDummy1
    {
        string Name { get; }
    }

    public interface IDummy2
    {
        int Size { get; }
    }

    public class Dummy1 : IDummy1
    {
        public string Name { get { return "Dummy1";  } }
    }

    public class Dummy2 : IDummy2
    {
        public int Size { get { return 42;  } }
    }

    public class DummyProcessorProvider : IFileProcessorProvider
    {

        public void ProcessFile(string resource_file, string transaction_id, System.Collections.Specialized.NameValueCollection requestParams)
        {
            throw new NotImplementedException();
        }
    }


}
