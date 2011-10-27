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

        public IFileUrlSignatureProvider CreateUrlSignatureProvider()
        {
            var provider = GetProvider<IFileUrlSignatureProvider>();
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
            });

            ObjectFactory.AssertConfigurationIsValid();
            _isWiredUp = true;
            return this;
        }

        public void ClearConfiguration()
        {
           ObjectFactory.Initialize( iex => { });
            _isWiredUp = false;
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
