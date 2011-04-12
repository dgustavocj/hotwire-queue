using System;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Providers
{
    public class ClassFactory
    {
        private readonly Logger _logger;

        public ClassFactory(Logger logger)
        {
            _logger = logger;
        }

        public T CreateInstance<T>(IAssemblyProvider assemblyProvider) where T : class
        {
            return CreateInstance<T>(assemblyProvider.AssemblyName, assemblyProvider.TypeName);
        }

        public T CreateInstance<T>(string assemblyName, string typeName) where T : class
        {
            try
            {
                var instance = (T) Activator.CreateInstance(assemblyName, typeName).Unwrap();
                if (instance == null)
                {
                    var msg = "Exception during creating " + typeof(T) + " instance obtained from Activator was null.";
                    _logger.Error(msg);
                    throw new ApplicationException(msg);
                }
                return instance;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception during creating " + typeof(T) + " instance.", ex);
                throw;
            }

        }

        public T CreateInstance<T>(string typeNameCommaAssembly) where T: class
        {
            string typeName = typeNameCommaAssembly.SplitComma()[0];
            string assemblyName = typeNameCommaAssembly.SplitComma()[1];
            return CreateInstance<T>(assemblyName, typeName);
        }
    }
}
