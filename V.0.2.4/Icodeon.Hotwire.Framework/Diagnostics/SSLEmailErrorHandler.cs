using System.Net;
using System.Net.Mail;
using Icodeon.Hotwire.Framework.Configuration;
using NLog;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public class SSLEmailErrorHandler : IExceptionHandler
    {
        private ISSLEMailErrorHandlerConfig _config;


        public SSLEmailErrorHandler()
        {
            var config = new SSLEmailErrorHandlerConfiguration().ReadConfig();
            _config = config;
        }

        public void HandleException(object sender, ExceptionEventArgs args)
        {
            var smtp = new SmtpClient();
            smtp.Timeout = _config.TimeoutSeconds*1000;
            
            foreach (var toAdress in _config.ToAddresses)
            {
                using (var message = new MailMessage(_config.FromAddress, toAdress))
                {
                    {
                        message.Subject = _config.SubjectLinePrefix + " " + args.Exception.Message;
                        message.Body = _config.SubjectLinePrefix + " " + args.Exception.Message + "\n"
                                       + "Section:" + args.Section + "\n"
                                       + "Request:" + args.Request;
                    }
                    smtp.Send(message);
                    // TODO: need to log this in a seperate email sent log.
                }
            }

        }
    }
}