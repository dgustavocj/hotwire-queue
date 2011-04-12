using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using NLog;

namespace Icodeon.Hotwire.Framework.Providers
{
    public class ProviderFactory
    {
        private readonly Logger _logger;

        public ProviderFactory()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }


        public ProviderFactory(Logger logger)
        {
            _logger = logger;
        }

        //TODO: consider caching the provider? use ninject singleton ?
        public IFileProcessorProvider CreateFileProcessor()
        {
            IAssemblyProvider assemblyInfo = FileProcessorSection.ReadConfig();
            var processorFactory = new ClassFactory(_logger).CreateInstance<IFileProcessorProvider>(assemblyInfo);
            return processorFactory;
        }

        public IConsumerProvider CreateConsumerProvider()
        {
            IAssemblyProvider assemblyInfo =new ConsumerProviderSection().ReadConfig();
            var consumer = new ClassFactory(_logger).CreateInstance<IConsumerProvider>(assemblyInfo);
            return consumer;
        }


        public IOAuthProvider CreateOauthProvider()
        {
            var assemblyProvider = OAuthProviderSection.ReadConfig();
            var oauthProvider = new ClassFactory(_logger).CreateInstance<IOAuthProvider>(assemblyProvider);
            return oauthProvider;
        }

        public IHttpClientProvider CreateHttpClient()
        {
            var assemblyInfo = new HttpClientProviderSection().ReadConfig();
            var httpClientWrapper = new ClassFactory(_logger).CreateInstance<IHttpClientProvider>(assemblyInfo);
            return httpClientWrapper;
        }
    }
}
