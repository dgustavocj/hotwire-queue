using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.TestFramework.Mocks;

namespace Icodeon.Hotwire.TestFramework
{
    public static class MockHttpWriterAssertions
    {
        /// <summary>
        /// Should be unauthorized
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static MockHttpWriter ShouldBe401UnAuthorised(this IHttpResponsableWriter response)
        {
            MockHttpWriter writer = response as MockHttpWriter;
            if (writer==null)
            {
                throw new ArgumentNullException("the writer is not a mock writer, cannot use these assertions here. A mock writer gives us access to the actual text written.");
            }
            if (writer.StatusCode != 401)
            {
                throw new ApplicationException("Response was not 401:  (Unauthorized) response code was " + response.StatusCode.ToString() + ". Response content was " + writer.GetWriterLines());
            }
            return writer;
        }

        private static string GetWriterLines(this MockHttpWriter writer)
        {
            return string.Join("\n", writer.Lines);
        }
    }

}
