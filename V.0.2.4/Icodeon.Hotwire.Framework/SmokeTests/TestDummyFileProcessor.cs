using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Framework.SmokeTests
{
    public static class TestDummyFileProcessor
    {
        public static void RunTest(HttpApplication context)
        {
            try
            {
                var config = FileProcessorSection.ReadConfig();
                // Try catch around this to make sure we managed to create assembly!
                var processor = Activator.CreateInstance(config.AssemblyName, config.TypeName).Unwrap() as IFileProcessorProvider;
                var nv = new NameValueCollection();

                string oauth_consumer_key = "dummy_oauth_consumer_key";
                string resource_id = "dummy_resource_id";
                string user_id = "dummy_user_id";
                string test_file = "test.zip";
                string transaction_id = "dummy_transaction_id";

                nv.Add("oauth_consumer_key", oauth_consumer_key);
                nv.Add("resource_id", resource_id);
                nv.Add("user_id", user_id);
                processor.ProcessFile(test_file, transaction_id, nv);

                context.Response.Write("<h1>/Tests/TestDummyFileProcessor</h1><br>");
                context.Response.Write("<h3>" + DateTime.Now.ToUniversalTime() + "</h3>");
                string template = @"
<table cellpadding='2' border='1'>
    <tr><td>oauth_consumer_key</td><td>{0}</td></tr>
    <tr><td>resource_id</td><td>{1}</td></tr>
    <tr><td>user_id</td><td>{2}</td></tr>
    <tr><td>test_file</td><td>{3}</td></tr>
    <tr><td>transaction_id</td><td>{4}</td></tr>
</table>";
                context.Response.Write(string.Format(template, oauth_consumer_key, resource_id, user_id, test_file, transaction_id));
                context.CompleteRequest();
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = ex.Message;
                context.Response.Write("<h1>Error:" + ex.Message + @"</h1><br/><pre>" + ex.StackTrace + "</pre>");
                context.CompleteRequest();
            }

        }
    }
}
