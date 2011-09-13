using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Security
{
    public interface IRequestAuthenticatorFactory
    {
        IAuthenticateRequest GetRequestAuthenticator(SecurityType endpointAuthorisation);
    }
}