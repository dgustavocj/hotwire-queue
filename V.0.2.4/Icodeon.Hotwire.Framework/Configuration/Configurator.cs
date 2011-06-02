using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Icodeon.Hotwire.Contracts;
using StructureMap;

namespace Icodeon.Hotwire.Framework.Configuration
{

    public class Configurator 
    {


        public virtual void AutoWireUpProviders()
        {
            
            ObjectFactory.Initialize(r => r.Scan(x =>
            {
                x.TheCallingAssembly();
                x.AssembliesFromApplicationBaseDirectory();
                x.AddAllTypesOf<IHttpClientProvider>();
            }));
            
        }

    
        public string GetDebugListingOfObjectFactoryRegistrations()
        {
            return ObjectFactory.WhatDoIHave();
        }
    }
}
