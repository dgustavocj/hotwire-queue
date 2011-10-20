using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Icodeon.Hotwire.TestFramework
{
    public class ServiceHelper
    {
        private readonly string _serviceName;

        public ServiceHelper(string serviceName)
        {
            _serviceName = serviceName;
        }

        public void UninstallIfInstalled()
        {
            throw new NotImplementedException();
        }

        private void InstallService()
        {
            throw new NotImplementedException();
        }

        public bool Installed
        {
            get
            {
                try
                {
                    var controller = new ServiceController(_serviceName);
                    string name = controller.DisplayName;
                    return true;
                }
                catch (InvalidOperationException op)
                {
                    if (op.Message.Contains("Service does not exist")) return false;
                    throw;
                }


            }
        }


    }
}
