using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Contracts.Enums;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Security
{
    public interface IRequestAuthenticatorFactory
    {
        IAuthenticateRequest GetRequestAuthenticator(SecurityType endpointAuthorisation);
    }
}