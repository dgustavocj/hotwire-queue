using System.IO;
using NLog;

namespace Icodeon.Hotwire.Framework.Deployment
{
    public class VersionedSiteManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _rootPath;
        private readonly string _domainName;
        private readonly int _port;
        private readonly string _applicationPool;

        private string VersionedName(string version)
        {
            return string.Format("{0}{1}", NamePrefix, version);
        }

        public virtual string NamePrefix
        {
            get
            {
                return string.Format("{0}_v", _domainName);    
            }
            
        }

        public VersionedSiteManager(string rootPath, string domainName, string applicationPool,int port)
        {
            _rootPath = rootPath;
            _domainName = domainName;
            _port = port;
            _applicationPool = applicationPool;
        }

        public void GoLive(string version)
        {
            _logger.Debug("GoLive('{0}')", version);
            var dm = new DeployManager();
            dm.DeleteApplicationPoolIfExists(_applicationPool); 
            dm.DeleteAllSitesStartingWith(NamePrefix);
            dm.CreateApplicationPoolIfNotExist(_applicationPool);
            string path = Path.Combine(_rootPath, VersionedName(version));
            dm.CreateSite(VersionedName(version),_domainName,_applicationPool, path, _port);
        }
    }
}