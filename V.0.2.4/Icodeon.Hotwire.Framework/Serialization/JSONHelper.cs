using System;
using System.IO;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;
using System.Runtime.Serialization.Json;

namespace Icodeon.Hotwire.Framework.Serialization
{
    public static class JSONHelper
    {
        public static string Serialize<T>(T obj)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            using(var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                string retVal = Encoding.UTF8.GetString(ms.ToArray());
                return retVal;
            }
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using(var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer( obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                ms.Dispose();
                return obj;
            }
        }

        public static void WriteJsonResponse<T>(this IHttpResponsableWriter writer, T src)
        {
            string json = Serialize(src);
            writer.ContentType = "application/json; charset=utf-8";
            writer.Write(json);
        }
    }
}