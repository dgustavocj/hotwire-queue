using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class NotWiredUpException : ApplicationException
    {
        public NotWiredUpException() : base("The provider has not been wired up before use. Please wire up the provider at application start.")
        {

        }
    }
}
