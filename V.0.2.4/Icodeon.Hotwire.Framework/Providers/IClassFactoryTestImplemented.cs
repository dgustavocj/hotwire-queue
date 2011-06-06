using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Providers
{
    // provided here as part of the framework so that you can run live testing on your IOC framework.
    // see Icodeon.Hotwire.Tests.ProviderFactoryTests.cs
    public interface IClassFactoryTestImplemented
    {
        string Message();
    }

    public class DefaultForClassFactoryImplemented : IClassFactoryTestImplemented
    {

        public string Message()
        {
            throw new ApplicationException("The unit test should have implemented a class, and the class factory should be returning that instance over this one. If this class is returned, then either the unit test project does not have an implementation of IClassFactoryTestImplemented, or there is a problem with the IOC / ClassFactory.");
        }
    }

}
