using System;
using System.IO;
using System.Linq;
using System.Threading;
using Icodeon.Hotwire.Framework.Diagnostics;
using Microsoft.Web.Administration;
using NLog;

namespace Icodeon.Hotwire.Framework.Deployment
{
    public class DeployManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        // NOW USING DELETE INSTEAD OF STOP FOR DEPLOYMENTS! MORE RELIABLE, NO WAITING FOR PROCESSING TO COMPLETE 
        //public bool StopAppPoolIfExist(string poolName, bool waitForPoolToStop, int msBetweenWaits, int maxTries)
        //{

            //using (ServerManager manager = new ServerManager())
            //{
            //    var pool = manager.ApplicationPools[poolName];
            //    if (pool == null) return false;
            //    pool.Stop();
            //    if (waitForPoolToStop && (pool.State == ObjectState.Stopping))
            //    {
            //        int cnt = 0;
            //        while(pool.State==ObjectState.Stopping)
            //        {
            //            Thread.Sleep(msBetweenWaits);
            //            if(++cnt>maxTries) throw new ApplicationException("timed out waiting for ApplicationPool '" + poolName + "' to stop.");
            //        }
            //        return true;
            //    }
            //    if (pool.State == ObjectState.Stopped) return true;
            //    if (pool.State==ObjectState.Unknown) throw new ApplicationException("ApplicationPool '" + poolName + "' is in an unknown state. Unable to Stop.");
            //    // will (should) never get to line below
            //    throw new  Exception("Unable to stop application pool '" + poolName + "'.");
            //}
        //}

        // START IS INSTANT, NO NEED FOR COMPLICATED SCRIPTS TO WAIT UNTIL ...
        // Additional checks are here in case something else kicks the server into some wierd state.
        public void StartAppPoolMustExist(string poolName, bool waitForPoolToStart, int msBetweenWaits, int maxTries)
        {
            _logger.Debug("StartAppPoolMustExist({0},...)",poolName);
            
            using (ServerManager manager = new ServerManager())
            {
                var pool = manager.ApplicationPools[poolName];

                if (pool == null) throw new LoggedException( _logger,"Could not find AppPool '" + poolName + "'.");
                pool.Start();
                if (pool.State == ObjectState.Started) return;
                if (waitForPoolToStart && (pool.State == ObjectState.Starting))
                {
                    int cnt = 0;
                    while (pool.State == ObjectState.Starting)
                    {
                        Thread.Sleep(msBetweenWaits);
                        if (++cnt > maxTries) throw new LoggedException(_logger, "timed out waiting for ApplicationPool '" + poolName + "' to start.");
                    }
                    return;
                }
                if (pool.State == ObjectState.Started) return;
                if (pool.State == ObjectState.Unknown) throw new LoggedException(_logger, "ApplicationPool '" + poolName + "' is in an unknown state. Unable to Start.");
                // will (should) never get to line below
                throw new LoggedException(_logger, "Unable to start application pool '" + poolName + "'.");
            }
        }



        public void CreateApplicationPoolIfNotExist(string poolName)
        {
            _logger.Debug("CreateApplicationPoolIfNotExist('{0}')",poolName);
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
            _logger.Debug("DeleteApplicationPoolIfExists('{0}')",poolName);
            _logger.LoggedExecution(()=> {
                                            using (ServerManager manager = new ServerManager())
                                            {
                                                var pool = manager.ApplicationPools[poolName];
                                                if (pool != null)
                                                {
                                                    _logger.Trace("removing pool:{0}:{1}",pool.Name,pool.State);
                                                    manager.ApplicationPools.Remove(pool);
                                                    // have to call commitchanges otherwise it's not deleted.
                                                    manager.CommitChanges();
                                                    // apparently we don't have to wait until removed
                                                }
                                            }
                                        });
        }





        // do I need to assert appropriate permissions for this?
        // must test what happens if I don't run with right permissions so that I get a suitable error message.
        public void CreateSite(string websiteName, string domainName, string applicationPoolName, string path, int port)
        {
            if (!Directory.Exists(path)) throw new ArgumentOutOfRangeException("Could not find path, path expected was:" + path);
            using (ServerManager manager = new ServerManager())
            {
                if (manager.ApplicationPools[applicationPoolName] == null)
                    throw new LoggedException(_logger, "ApplicationPool '" + applicationPoolName + "' does not exist. It must be created before calling CreateSite.");
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
            if (existingBinding != null) throw new LoggedException(_logger, "An http binding with the same (ip), port and host header already exists.");

            Binding newBinding = site.Bindings.CreateElement();
            newBinding.Protocol = "http"; 
            newBinding.BindingInformation = bind;
            site.Bindings.Add(newBinding);
        }

        public void DeleteAllSitesStartingWith(string testDomainName)
        {
            _logger.Debug("DeleteAllSitesStartingWith('{0}')",testDomainName);
            _logger.LoggedExecution(() =>
                                        {
                                            using (ServerManager manager = new ServerManager())
                                            {
                                                _logger.Trace("current domain name list:{0}", manager.DomainNameList());
                                                var sitesToDelete = manager.Sites.Where(s => s.Name.StartsWith(testDomainName)).Select( s => s.Name).ToArray();
                                                _logger.Trace("deleting sites:({0})",string.Join(",",sitesToDelete));
                                                foreach (string siteName in sitesToDelete)
                                                {
                                                    _logger.Trace("deleting '{0}'", manager.DomainNameList());
                                                    manager.Sites[siteName].Delete();
                                                }
                                                manager.CommitChanges();
                                            }
                                        });
        }

    }
}