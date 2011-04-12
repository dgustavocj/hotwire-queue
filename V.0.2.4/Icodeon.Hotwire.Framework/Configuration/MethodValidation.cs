using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public enum MethodValidation
    {
        /// <summary>
        /// Will be faster and less resource intensive as the string checking will be avoided on most requests. 
        /// </summary>
        /// <remarks>
        /// The side effect is that the httpModule will return 404 if the uri is correct and the method is not configured.
        /// </remarks>
        beforeUriValidation,

        /// <summary>
        /// This is more technically correct but requires that full UriTemplate matching is performed on every single http request. Will return a more useful 405 Method Not allowed. 
        /// </summary>
        afterUriValidation
    }
}
