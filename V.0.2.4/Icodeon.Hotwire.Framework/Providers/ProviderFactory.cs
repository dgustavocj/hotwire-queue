using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Diagnostics;
using StructureMap;

namespace Icodeon.Hotwire.Framework.Providers
{
    public class ProviderFactory
    {
        private readonly LoggerBase _logger;

        public ProviderFactory(LoggerBase logger)
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
            var httpClient = ObjectFactory.GetInstance<IHttpClientProvider>();
            return httpClient;
        }
    }
}
