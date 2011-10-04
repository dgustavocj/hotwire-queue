using System;
using System.IO;
using System.Linq;
using Microsoft.Web.Administration;

namespace Icodeon.Hotwire.Framework.Deployment
{
    public class DeployManager
    {

        public void CreateApplicationPoolIfNotExist(string poolName)
        {
            using (ServerManager manager = new ServerManager())
            {
                var pool = manager.ApplicationPools[poolName];
                if (pool == null)
                {
                    pool = manager.ApplicationPools.Add(poolName);
                    pool.AutoStart = true;
                }
                manager.CommitChanges();
            }
        }



        // do I need to assert appropriate permissions for this?
        // must test what happens if I don't run with right permissions so that I get a suitable error message.
        public void CreateSite(string websiteName, string domainName, string applicationPoolName, string path, int port)
        {
            if (!Directory.Exists(path)) throw new ArgumentOutOfRangeException("Could not find path, path expected was:" + path);
            using (ServerManager manager = new ServerManager())
            {
                CreateApplicationPoolIfNotExist(applicationPoolName);
                // Consider? Dow we need to create site with default started = false so that we can change a few things before it "starts" automatically?
                Site site = manager.Sites.Add(websiteName, path, port);
                // application pool must already exist.
                site.Applications[0].ApplicationPoolName = applicationPoolName;
                CreateHttpProtocolBinding(site, domainName,port);
                //manager.ApplicationPools[applicationPoolName].Recycle();
                //site.Start();
                manager.CommitChanges();
            }
            
        }

        void CreateHttpProtocolBinding(Site site, string domainName, int port)
        {
            string bind = string.Format("*:{0}:{1}", port, domainName); // the * is for ALL IPs
            var existingBinding = site.Bindings.FirstOrDefault(b => b.Protocol == "http" && b.BindingInformation == bind);
            if (existingBinding != null) throw new Exception("An http binding with the same (ip), port and host header already exists.");

            Binding newBinding = site.Bindings.CreateElement();
            newBinding.Protocol = "http";
            newBinding.BindingInformation = bind;
            site.Bindings.Add(newBinding);
        }

        public void DeleteAllSitesStartingWith(string appPoolName, string testDomainName)
        {
            using (ServerManager manager = new ServerManager())
            {
                // ensure there are no worker processes running ( i.e. if we've recently made http requests

                var sites = manager.Sites.Where(s => s.Name.StartsWith(testDomainName));
                foreach (Site site in sites)
                {
                    // stop the "test" app pool
                    site.Delete();
                }
                manager.CommitChanges();
            }

        }
    }
}