using System;
using System.Linq;
using System.Reflection;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;
using StructureMap;

namespace Icodeon.Hotwire.Framework.Providers
{
    public class ProviderFactory 
    {
        public ProviderFactory()
        {
        }

        public IFileProcessorProvider CreateFileProcessor()
        {
            try
            {
                var provider = GetProvider<IFileProcessorProvider>();
                return provider;
            }
            // dont throw structuremap exceptions, this will allow us to replace structuremap with ninject (or anything else) later if we need.
            catch (StructureMapException sex)
            {
                throw new AmbiguousMatchException(sex.Message);
            }
        }

        public IConsumerProvider CreateConsumerProvider()
        {
            var provider = GetProvider<IConsumerProvider>();
            return provider;
        }

        public IOAuthProvider CreateOauthProvider()
        {
            var provider = GetProvider<IOAuthProvider>();
            return provider;
        }

        public IHttpClientProvider CreateHttpClient()
        {
            var httpClient = GetProvider<IHttpClientProvider>();
            return httpClient;
            
        }


        // Make sure that autoWireUpProviders is called onInit() on global.asax in websites!

        public ProviderFactory AutoWireUpProviders()
        {
            ObjectFactory.Initialize(r =>
            {
                r.For<HotLogger>().Use<NullLogger>();
                r.For<LoggerBase>().Use<NullLogger>();
                r.Scan(x =>
                        {
                            x.TheCallingAssembly();
                            x.AssembliesFromApplicationBaseDirectory();

                            x.AddAllTypesOf<IFileProcessorProvider>();
                            x.AddAllTypesOf<IConsumerProvider>();
                            x.AddAllTypesOf<IOAuthProvider>();
                            x.AddAllTypesOf<IHttpClientProvider>();
                            x.AddAllTypesOf<IClassFactoryNotImplemented>();
                            x.AddAllTypesOf<IClassFactoryTestImplemented>();

                            // there's probably a way to do this (code below) with some linq query, which will avoid having to actually create instance
                            // of any user implemented providers. Hotwire developers will need to know that certain providers will be instantiated 
                            // once during configuration, which might change their expected first usage.

                            x.ExcludeType<LoggingFileProcessorProvider>();
                            x.ExcludeType<ConsumerProvider>();
                            x.ExcludeType<DefaultForClassFactoryImplemented>();
                            x.ExcludeType<DefaultForClassFactoryNotImplemented>();
                        });
            });
            ProvideDefaultIfProviderNotRegisteredFor<IFileProcessorProvider, LoggingFileProcessorProvider>();
            ProvideDefaultIfProviderNotRegisteredFor<IConsumerProvider, ConsumerProvider>();
            ProvideDefaultIfProviderNotRegisteredFor<IClassFactoryNotImplemented, DefaultForClassFactoryNotImplemented>();
            ProvideDefaultIfProviderNotRegisteredFor<IClassFactoryTestImplemented, DefaultForClassFactoryImplemented>();
            ObjectFactory.AssertConfigurationIsValid();
            return this;
        }

        internal void ProvideDefaultIfProviderNotRegisteredFor<TProvider,TConcrete>() where TConcrete : TProvider
        {
            try
            {
                var provider = ObjectFactory.GetInstance<TProvider>();
            }
            // NB! Don't use Initialize here (which will wipe existing), use Configure (which adds to existing configuration.)
            catch (StructureMapException)
            {
                ObjectFactory.Configure(x => x.For<TProvider>().Use<TConcrete>());
            }
        }

        internal T GetProvider<T>()
        {
            try
            {
                T provider = ObjectFactory.GetInstance<T>();
                return provider;
            }
            catch (StructureMapException sex)
            {
                throw new AmbiguousMatchException(sex.Message + ":" + ObjectFactory.WhatDoIHave(), sex);
            }
        }

        public string GetDebugListingOfObjectFactoryRegistrations()
        {
            return ObjectFactory.WhatDoIHave();
        }



    }
}
