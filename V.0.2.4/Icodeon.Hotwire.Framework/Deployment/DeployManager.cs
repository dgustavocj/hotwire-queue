using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Web.Administration;
using NLog;

namespace Icodeon.Hotwire.Framework.Deployment
{
    public class DeployManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Returns false if not exist, true if exists and now stopped.
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="waitForPoolToStop"></param>
        /// <returns></returns>
        public bool StopAppPoolIfExist(string poolName, bool waitForPoolToStop, int msBetweenWaits, int maxTries)
        {
            using (ServerManager manager = new ServerManager())
            {
                var pool = manager.ApplicationPools[poolName];
                if (pool == null) return false;
                pool.Stop();
                if (waitForPoolToStop && (pool.State == ObjectState.Stopping))
                {
                    int cnt = 0;
                    while(pool.State==ObjectState.Stopping)
                    {
                        Thread.Sleep(msBetweenWaits);
                        if(++cnt>maxTries) throw new ApplicationException("timed out waiting for ApplicationPool '" + poolName + "' to stop.");
                    }
                    return true;
                }
                if (pool.State == ObjectState.Stopped) return true;
                if (pool.State==ObjectState.Unknown) throw new ApplicationException("ApplicationPool '" + poolName + "' is in an unknown state. Unable to Stop.");
                // will (should) never get to line below
                throw new  Exception("Unable to stop application pool '" + poolName + "'.");
            }
        }

        public void StartAppPoolMustExist(string poolName, bool waitForPoolToStart, int msBetweenWaits, int maxTries)
        {
            using (ServerManager manager = new ServerManager())
            {
                var pool = manager.ApplicationPools[poolName];
                if (pool == null) throw new ArgumentNullException("Could not find AppPool '" + poolName + "'.");
                pool.Start();
                if (pool.State == ObjectState.Started) return;
                if (waitForPoolToStart && (pool.State == ObjectState.Starting))
                {
                    int cnt = 0;
                    while (pool.State == ObjectState.Starting)
                    {
                        Thread.Sleep(msBetweenWaits);
                        if (++cnt > maxTries) throw new ApplicationException("timed out waiting for ApplicationPool '" + poolName + "' to start.");
                    }
                    return;
                }
                if (pool.State == ObjectState.Started) return;
                if (pool.State == ObjectState.Unknown) throw new ApplicationException("ApplicationPool '" + poolName + "' is in an unknown state. Unable to Start.");
                // will (should) never get to line below
                throw new Exception("Unable to start application pool '" + poolName + "'.");
            }
        }



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


        public void DeleteApplicationPoolIfExists(string poolName)
        {
            using (ServerManager manager = new ServerManager())
            {
                var pool = manager.ApplicationPools[poolName];
                if (pool != null)
                {
                    manager.ApplicationPools.Remove(pool);
                    manager.CommitChanges();
                }
            }
        }





        // do I need to assert appropriate permissions for this?
        // must test what happens if I don't run with right permissions so that I get a suitable error message.
        public void CreateSite(string websiteName, string domainName, string applicationPoolName, string path, int port)
        {
            if (!Directory.Exists(path)) throw new ArgumentOutOfRangeException("Could not find path, path expected was:" + path);
            using (ServerManager manager = new ServerManager())
            {
                if (manager.ApplicationPools[applicationPoolName] == null)
                    throw new ApplicationException("ApplicationPool '" + applicationPoolName + "' does not exist. It must be created before calling CreateSite.");
                Site site = manager.Sites.Add(websiteName, path, port);
                site.Applications[0].ApplicationPoolName = applicationPoolName;
                site.ServerAutoStart = true;
                CreateHttpProtocolBinding(site, domainName,port);
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

        public void DeleteAllSitesStartingWith(string testDomainName)
        {
            using (ServerManager manager = new ServerManager())
            {
                // ensure there are no worker processes running ( i.e. if we've recently made http requests

                var sites = manager.Sites.Where(s => s.Name.StartsWith(testDomainName));
                foreach (Site site in sites)
                {
                    site.Delete();
                }
                manager.CommitChanges();
            }

        }
    }
}