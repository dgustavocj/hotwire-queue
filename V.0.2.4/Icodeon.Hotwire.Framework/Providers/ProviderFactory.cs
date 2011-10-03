using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Http;
using Icodeon.Hotwire.Framework.Security;
using Icodeon.Hotwire.Framework.Utils;
using StructureMap;

namespace Icodeon.Hotwire.Framework.Providers
{
    public class ProviderFactory
    {
        private static bool _isWiredUp = false;

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

        public ProviderFactory WireUp<TSrc,TDest>()  where TDest : TSrc
        {
            _isWiredUp = true;
            ObjectFactory.Configure(x => x.For<TSrc>().Use<TDest>());
            return this;
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

        public IDateTime CreateDateTimeProvider()
        {
            var dateTime = GetProvider<IDateTime>();
            return dateTime;
        }



        // Make sure that autoWireUpProviders is called onInit() on global.asax in websites!


        public ProviderFactory AutoWireUpProviders()
        {
            ObjectFactory.Initialize(r =>
            {
                r.For<IDateTime>().Use<DateTimeWrapper>();
                r.For<HotLogger>().Use<NullLogger>();
                r.For<LoggerBase>().Use<NullLogger>();
                // Should use a singleton "per request" for hotwire context
                // for now it's not necessary since the current usage will only hit the database once per request
                // if this increases and/or gets more complicated then this can be adjusted.
                r.For<ISimpleMacDAL>().Use(new SimpleMacDal(new DateTimeWrapper(), new HotwireContext(ConnectionStringManager.HotwireConnectionString)));
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
            ProvideDefaultIfProviderNotImplementedFor<IFileProcessorProvider, LoggingFileProcessorProvider>();
            ProvideDefaultIfProviderNotImplementedFor<IConsumerProvider, ConsumerProvider>();
            ProvideDefaultIfProviderNotImplementedFor<IClassFactoryNotImplemented, DefaultForClassFactoryNotImplemented>();
            ProvideDefaultIfProviderNotImplementedFor<IClassFactoryTestImplemented, DefaultForClassFactoryImplemented>();
            ProvideDefaultIfProviderNotImplementedFor<IDateTime, DateTimeWrapper>();
            ObjectFactory.AssertConfigurationIsValid();
            _isWiredUp = true;
            return this;
        }

        public void ClearConfiguration()
        {
           ObjectFactory.Initialize( iex => { });
            _isWiredUp = false;
        }

        internal void ProvideDefaultIfProviderNotImplementedFor<TProvider,TConcrete>() where TConcrete : TProvider
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
            if (!_isWiredUp) throw new NotWiredUpException();
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
            if (!_isWiredUp) throw new NotWiredUpException();
            return ObjectFactory.WhatDoIHave();
        }



    }
}
