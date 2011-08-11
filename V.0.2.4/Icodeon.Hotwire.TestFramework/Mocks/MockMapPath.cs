using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockMapPath : IMapPath
    {
        public string PathToReturn { get; set; }

        public string MapPath(string path)
        {
            return PathToReturn;
        }
    }
}
