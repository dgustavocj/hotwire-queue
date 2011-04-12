using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.Framework.Serialization;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class ContextHelper
    {
        /// <summary>
        /// NB! While exception is "handled", Context.CompleteRequest is not called. maybe will be changed to include here later, but not for now. For now, caller must call CompleteRequest (suggested use is to be placed in a finally block).
        /// </summary>
        /// <param name="httpWriter"></param>
        /// <param name="ex"></param>
        public static void HandleException(IHttpResponsableWriter httpWriter, Exception ex)
        {
            // you can't serialize all exceptions with the DataContract Serialiser, so a simplified wrapper class that
            // is serialisable is required, hence the use of HotwireExceptionDTO below.
            WriteJson(httpWriter,new HotwireExceptionDTO(ex), HttpStatusCode.InternalServerError);
        }

        public static void WriteJson<T>(IHttpResponsableWriter httpWriter, T retval, HttpStatusCode statusCode)
        {
            httpWriter.StatusCode = (int)statusCode;
            httpWriter.ContentType = "application/json";
            string json = JSONHelper.Serialize<T>(retval);
            httpWriter.Write(json);
        }

        public static void WriteHtml<T>(IHttpResponsableWriter httpWriter, string src, string title, NLog.Logger logger)
        {
            var html = new MediaTypeFactory().Html;
            httpWriter.StatusCode = (int)HttpStatusCode.Accepted;
            httpWriter.ContentType = html.ContentType;
            logger.Trace("Setting response ContentType to {0} and writing response", html.ContentType);
            httpWriter.Write(string.Format(htmlTemplate,title,src));
        }

        public static void WriteHtml<T>(IHttpResponsableWriter httpWriter, T src, HttpStatusCode code, NLog.Logger logger)
        {
           // WriteResponseAsSupportedMediaElse415(httpWriter,src,HttpStatusCode.Accepted, ".html",title,logger);
            WriteMediaResponse(httpWriter, new MediaTypeFactory().Html, src, code, logger);
        }

            private const string htmlTemplate =
@"<html>
    <head>
    </head>
    <body>
    <div class='hotwire-object'>
        <textarea rows='20' cols='120'>
        {0}
        </textarea>
    </div>
    </body>
</html>";


        public static void WriteMediaResponse<T>(IHttpResponsableWriter httpWriter, IMediaInfo media, T retval, HttpStatusCode statusCode, NLog.Logger logger)
        {
            WriteMediaResponse<T>(httpWriter, media, retval, (int) statusCode, logger);
        }

        public static void WriteMediaResponse<T>(IHttpResponsableWriter httpWriter, IMediaInfo media, T retval, int statusCode, NLog.Logger logger)
        {
            logger.Trace("WriteMediaResponse<T>(httpWriter,IMediaInfo[{0}],HttpStatusCode[{0}],logger", statusCode);
            httpWriter.StatusCode = (int)statusCode;
            httpWriter.ContentType = media.ContentType;
            logger.Trace("Setting response ContentType to {0} and writing response", media.ContentType);

            switch (media.Type)
            {
                case (eMediaType.html):
                    string jsonHtml = JSONHelper.Serialize<T>(retval);
                    string html = string.Format(htmlTemplate, jsonHtml);

                    httpWriter.Write(html);
                    break;

                case (eMediaType.text):
                    if (retval is string)
                    {
                        // JSON serialiser adds quotes around strings, which we don't want if the 
                        // type is string which can be assigned to strings. 
                        // We dont want to have to strip off quotes.
                        httpWriter.Write(retval.ToString());
                        break;
                    }
                    string jsonText  = JSONHelper.Serialize<T>(retval);
                    httpWriter.Write(jsonText);
                    break;

                case (eMediaType.json):
                    string json = JSONHelper.Serialize<T>(retval);
                    httpWriter.Write(json);
                    break;

                case (eMediaType.xml):
                    string xml = XmlHelper.Serialize<T>(retval);
                    httpWriter.Write(xml);
                    break;
                


                default:
                    httpWriter.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                    break;
            }

        }


    }
}
