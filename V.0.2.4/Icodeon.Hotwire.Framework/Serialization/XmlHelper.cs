using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Serialization
{
    public static class XmlHelper
    {
        public static string Serialize<T>(T obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            var settings = new XmlWriterSettings() { Indent = true,Encoding = Encoding.UTF8,OmitXmlDeclaration = true };
            
            using (var ms = new MemoryStream())
            {
                using (var w = XmlWriter.Create(ms,settings))
                {
                    serializer.WriteObject(ms, obj);
                    string retVal = Encoding.UTF8.GetString(ms.ToArray());
                    return retVal;
                } // using
            } // using
        }

    }
}