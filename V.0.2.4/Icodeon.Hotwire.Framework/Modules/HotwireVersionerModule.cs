using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class HotwireVersionerModule  : IHttpModule 
    {

        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += delegate
                                        {
                                            context.Response.AddHeader(Parameters.Ver0_1.Headers.hotwire_version_framework, AssemblyHelper.FrameworkVersion);
                                            context.Response.AddHeader(Parameters.Ver0_1.Headers.hotwire_version_service, AssemblyHelper.ServiceVersion);
                                        };
        }

        

    }
}