using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.MediaTypes
{
    public class MediaTypeFactory
    {

        public IMediaInfo JSON { get; private set; }
        public IMediaInfo Html { get; private set; }
        public IMediaInfo Text { get; private set; }
        public IMediaInfo Xml { get; private set; }

        public MediaTypeFactory()
        {
            // not the best link possible to media types, it's a start...
            // http://xhtml.com/en/xhtml/media-types-how-the-web-works/

            JSON = new MediaInfo(eMediaType.json, "application/json",new []{ ".json"});
            Html = new MediaInfo(eMediaType.html, "text/html", new []{".html",".htm" } );
            Text = new MediaInfo(eMediaType.text, "text/plain", new []{".txt" });
            Xml = new MediaInfo(eMediaType.xml, "text/xml", new[] { ".xml" });
        }

        public IMediaInfo this[eMediaType format]
        {
            get
            {
                switch (format)
                {
                    case eMediaType.html: return Html;
                    case eMediaType.text: return Text;
                    case eMediaType.json: return JSON;
                    case eMediaType.xml: return Xml;
                    default: throw new ArgumentOutOfRangeException(format.ToString());
                }
            }
        }


    }



}
