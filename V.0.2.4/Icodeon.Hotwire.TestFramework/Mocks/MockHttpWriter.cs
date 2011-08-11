using System.Collections.Generic;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockHttpWriter : IHttpResponsableWriter
    {
        public string ContentType { get; set; }

        public List<string> Lines = new List<string>();

        public void Write(string s)
        {
            Lines.Add(s);
        }
        public int StatusCode { get; set; }
    }
}