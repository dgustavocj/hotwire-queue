using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using FluentAssertions;
using NLog;

namespace Icodeon.Hotwire.TestFramework
{
    public sealed class LMHosts : IDisposable
    {

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private bool _hasModifiedHosts = false;
        private bool _backedUp = false;


        // looks like I dont need to flush dns, changes appear immediately.
        // will be good to see if they are cached?
        //[DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        //private static extern UInt32 DnsFlushResolverCache();
        //public uint FlushDNS()
        //{
        //    UInt32 result = DnsFlushResolverCache();
        //    return (uint)result;
        //}

        public string[] Lines;


        private string _hostsPath;
        private string _backupPath;

        public LMHosts(bool createBackup)
        {
            _hostsPath = Environment.ExpandEnvironmentVariables(@"%windir%\system32\drivers\etc\hosts");
            _backupPath = Environment.ExpandEnvironmentVariables(@"%windir%\system32\drivers\etc\hosts-bak");
            Lines = File.ReadAllLines(_hostsPath);
            if (createBackup)
            {
                CreateBackup();
                _backedUp = true;
            }
        }


        private void CreateBackup()
        {
            // don't overwrite so that you get an error indicating the backup already exists.
            if (File.Exists(_backupPath)) throw new Exception("Hosts backup file already exists... that's a problem because it should not exist (should be cleaned up in event of any error, and could possibly indicate a problem with LMHosts.cs class!");
            _logger.Info("backing up the lmhosts file. Copy is:" + _backupPath);
            File.Copy(_hostsPath, _backupPath);
        }

        public void ShouldNotResolveHostName(string hostname)
        {
            Action action = () => System.Net.Dns.GetHostAddresses(hostname);
            action.ShouldThrow<SocketException>();
        }


        public void ShouldResolveToLocalhost(string hostname)
        {
            var adresses = System.Net.Dns.GetHostAddresses(hostname);
            var address = adresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            address.Should().Be("127.0.0.1");
        }

        private void RestoreBackup()
        {
            ThrowItsReadOnlyExceptionIfNotBackedUp("RestoreBackup()");
            File.Copy(_backupPath, _hostsPath, true);
        }

        private void ThrowItsReadOnlyExceptionIfNotBackedUp(string methodName)
        {
            if (!_backedUp) throw new ApplicationException("cannot call '" + methodName + "', because no backup was created, and LMHosts is in readonly mode.");
        }

        public void AppendLine(string line)
        {
            ThrowItsReadOnlyExceptionIfNotBackedUp("AppendLine()");
            _hasModifiedHosts = true;
            File.AppendAllText(_hostsPath,"\r\n"+ line);
        }

        public LMHosts AddHostEntryIfNotExist(string hostName, string ipAddress)
        {
            ThrowItsReadOnlyExceptionIfNotBackedUp("AddHostEntryIfNotExist()");
            if (!Lines.Contains(hostName))
            {
                _hasModifiedHosts = true;
                string hostLine = string.Format("\r\n{0}    {1}", ipAddress, hostName);
                File.AppendAllText(_hostsPath,hostLine);
            }
            return this;
        }

        

        private bool _disposed = false;

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        // do nothing : currently no managed objects to dispose
                    }
                    if (_hasModifiedHosts) RestoreBackup();
                    if (File.Exists(_backupPath)) File.Delete(_backupPath);

                    _disposed = true;
                }
            }
            catch (Exception ex)
            {
                // while a failure is bad...very bad, it should not take down the server as long as we're only using the LHMosts file to append #Include's or few non real and non live (test) dns names
                // the worst that should happen is that various testing will fail until the hosts file is restored. The bad side is that it's possible that
                // "other" testing could fail with strange side effects because a backup of the lmhosts file was not restored.
                _logger.LogException(LogLevel.Fatal,"Fatal error during LmHosts disposal, hosts files may be in an invalid state and it's possible host file backup may not have been restored! Please investigate.",ex);
                throw;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~LMHosts()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }


    }
}
