using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.TestAspNet
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
             new ProviderFactory().AutoWireUpProviders();


            // sample usage:
            // even if you will wire up your own providers, still call autowireUpProviders as that will wire up
            // defaults for all the providers that you do not provide.
            // new ProviderFactory().AutoWireUpProviders()
            //    .WireUp<IFileProcessorProvider, YourProcessorProvider>()
            //    .WireUp<IConsumerProvider, YourConsumerProvider>()
            //    .WireUp<IOAuthProvider, YourOAuthProvider>();

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
