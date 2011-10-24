using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Icodeon.Hotwire.TestFramework
{
    public class ServiceHelper
    {
        private readonly string _serviceName;
        private readonly string _assemblyName;
        private ServiceController _serviceController;

        public ServiceHelper(string serviceName, string assemblyName)
        {
            if (!IsUserAdministrator()) throw new ApplicationException("You must be member of 'admin' group to use ServiceHelper.");
            _serviceName = serviceName;
            _assemblyName = assemblyName;
            ServiceController sc = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == _serviceName);
        }

        public static bool IsInstalled(string serviceName)
        {
            var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
            return service != null && service.ServiceName == serviceName;
        }



        public static void InstallService(string assemblyName, string serviceName)
        {
            if (IsInstalled(serviceName)) return;
            ManagedInstallerClass.InstallHelper(new string[] { assemblyName });
        }

        public void UnInstallService()
        {
            if (!IsInstalled(_serviceName)) return;
            ManagedInstallerClass.InstallHelper(new string[] { _assemblyName, "/u" });
        }


        public void Start()
        {
            _serviceController.Start();
            int cnt = 0;
            while(_serviceController.Status == ServiceControllerStatus.StartPending)
            {
                Thread.Sleep(100 * cnt + 100); // wait longer and longer
                if (cnt++ > 5) throw new ApplicationException("timed out waiting for service to start, waited approximately 10 seconds.");
            }
        }

        public void Stop()
        {
            _serviceController.Stop();
            int cnt = 0;
            while (_serviceController.Status == ServiceControllerStatus.StopPending)
            {
                Thread.Sleep(100 * cnt + 100); // wait longer and longer
                if (cnt++ > 5) throw new ApplicationException("timed out waiting for service to stop, waited approximately 10 seconds.");
            }
            
        }

        private bool IsUserAdministrator()
        {
            try
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

}
