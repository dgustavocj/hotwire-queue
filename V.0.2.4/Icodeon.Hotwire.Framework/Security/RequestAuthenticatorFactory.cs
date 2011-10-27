using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Contracts.Enums;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Utils;
using StructureMap;

namespace Icodeon.Hotwire.Framework.Security
{
    //CONSIDER: this design makes it difficult for user of framework to extend (ie use their own custom authorization) without committing extensions to googlecode

    public class RequestAuthenticatorFactory : IRequestAuthenticatorFactory
    {
        //todo: should accept lambdas for constructing all the objects. (as dependancies)
        public IAuthenticateRequest GetRequestAuthenticator(SecurityType endpointAuthorisation)
        {
            switch (endpointAuthorisation)
            {
                case SecurityType.none:
                    return new NoSecurityRequestAuthenticator();
                case SecurityType.oauth:
                    var consumer = new ProviderFactory().CreateConsumerProvider();
                    var oAuthProvider = new ProviderFactory().CreateOauthProvider();
                    return new OAuthRequestAuthenticator(consumer, oAuthProvider);
                case SecurityType.localonly:
                    return new LocalOnlyRequestAuthenticator();
                case SecurityType.simpleMAC:
                    var dateTime = ObjectFactory.GetInstance<IDateTime>();
                    var guidSaltCache = ObjectFactory.GetInstance<ISimpleMacDAL>();
                    return new SimpleMacAuthenticator(dateTime, guidSaltCache);
                default:
                    throw new ArgumentOutOfRangeException("endpointAuthorisation");
            }
        }

    }
}
