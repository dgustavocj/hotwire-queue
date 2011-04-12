using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;
using System.ServiceModel;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework
{
    public class OAuthHelper
    {
        private readonly string _key;
        private readonly string _secret;
        private readonly TimeSpan _timeout;

        /// <summary>
        /// Warning : is dependant on the HttpContext! THrows 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="timeout"></param>
        public OAuthHelper(string key, string secret, TimeSpan timeout)
        {
            _key = key; // key is used to lookup the plain text key from storage.
            _secret = secret;
            _timeout = timeout;
        }

        internal const string OAUTH_NOT_SIGNED_PART_ERR_MSG = "signature";

        // convert this to a WCF attribute
        // if the attribute exists then the method needs to be OAuth signed.
        // process all exceptions, if Unauthorized access + any others then then create friendly responses.

        /// <summary>
        /// Validates that the HttpRequest has been OAuth signed. This includes the signature, the Nonce and the
        /// Timestamp constraints. This is only a validation of the request, so we
        /// do not do anything with the RequestToken that is generated.
        /// </summary>
        /// 
        /// <exception cref="System.ApplicationException">Thrown if there is some
        /// problem issusing a request token</exception> 
        /// <exception cref="System.ServiceModel.Web.WebFaultException">401 (Uauthorized) Thrown if the 
        /// Oauth is not signed.</exception> 
        public void ValidateHttpRequestHasbeenOAuthSigned_1_0(NameValueCollection requestParams)
        {
            try
            {
                //TODO: Finish tests and implementation!
                // CALL OAUTH LIBRARY HERE!
                // CHECK THE SPECIFIC ERROR MESSAGE TEXT FROM THE LIBRARY, so that code below is correct.

            }
            catch (ApplicationException ae)
            {
                if (ae.Message.ToLower().Contains(OAUTH_NOT_SIGNED_PART_ERR_MSG))
                        // to decide: whether to use WebFaultException or WebException.
                        // wait until there's a feature card for it, until then dont get sidetracked! Simplest is best

                    // 401!
                    throw new WebFaultException(HttpStatusCode.Unauthorized);
                //throw new WebException(ae.Message, WebExceptionStatus.UnknownError  HttpStatusCode.Unauthorized);
            }
            catch(Exception ex)
            {
                // hosted in IIS
                if (HttpContext.Current!=null) 
                    HttpContext.Current.Response.Write("Unhandled exception,'" + ex.Message + "', please see logfile.");
                else
                {
                    // self hosted in WCF with no asp.net compatibility
                }
                // both
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }
        }

    } // class
} // namespace