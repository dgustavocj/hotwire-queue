using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.MediaTypes
{
    public class MediaInfo : IMediaInfo
    {

        public MediaInfo(eMediaType type, string contentType, IEnumerable<string> extensions)
        {
            ContentType = contentType;
            Extensions = extensions;
            Type = type;
        }

        public string ContentType { get; private set; }
        public IEnumerable<string> Extensions { get; private set; }

        public bool IsMediaType(string rawUrl)
        {
            return (Extensions.Any(ext => rawUrl.EndsWith(ext)));
        }

        public eMediaType Type { get; private set; }
    }
}
