using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
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

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                ms.Dispose();
                return obj;
            }
        }

    }
}