using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Security
{
    public class RequestAuthenticatorFactory
    {
        public IAuthenticateRequest GetRequestAuthenticator(HotLogger logger, SecurityType endpointAuthorisation)
        {
            switch (endpointAuthorisation)
            {
                case SecurityType.none:
                    return new NoSecurityRequestAuthenticator();
                case SecurityType.oauth:
                    return new OAuthRequestAuthenticator(logger);
                case SecurityType.localonly:
                    return new LocalOnlyRequestAuthenticator();
                case SecurityType.simpleMAC:
                    return new SimpleMACAuthenticator();

                default:
                    throw new ArgumentOutOfRangeException("endpointAuthorisation");
            }
        }

    }
}
