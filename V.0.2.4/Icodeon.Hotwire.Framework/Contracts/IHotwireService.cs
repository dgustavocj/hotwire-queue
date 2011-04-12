using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [ServiceContract(Namespace = Constants.Namespaces.ICODEON_HOTWIRE_BETA_V0_2)]
    public interface IHotwireService
    {
        [Description("Returns the version number of the hotwire framework assembly.")]
        [OperationContract, WebGet(UriTemplate = Uris.Ver0_2.VersionFramework_GET, ResponseFormat = WebMessageFormat.Json)]
        string VersionFramework();

        [Description("Returns the version number of service host assembly.")]
        [OperationContract, WebGet(UriTemplate = Uris.Ver0_2.VersionServiceHost_GET , ResponseFormat = WebMessageFormat.Json)]
        string VersionServiceHost();

        [OperationContract, WebInvoke(Method = "POST", UriTemplate = Uris.Ver0_2.QueuedCartridges_xml_POST, 
            ResponseFormat = WebMessageFormat.Xml, 
            RequestFormat = WebMessageFormat.Xml
            )]
        QueuedResource Enqueue(string module, Stream body);
    }
}