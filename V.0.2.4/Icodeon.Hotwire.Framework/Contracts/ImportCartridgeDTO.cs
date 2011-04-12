using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Icodeon.Hotwire.Framework.Serialization;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [DataContract]
    public class ImportCartridgeDTO 
    {
        public ImportCartridgeDTO()
        {

        }

        [DataMember]
        public CartridgeDTO Cartridge { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string OAuthConsumerKey { get; set; }


        //TODO: Move to EnqueueRequestDTO

        //TODO: Move to EnqueueRequestDTO

        public static ImportCartridgeDTO ReadImportSettingsFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var dto = JSONHelper.Deserialize<ImportCartridgeDTO>(json);
            return dto;
        }

        public static void WriteImportSettingsFile(ImportCartridgeDTO src, string folder, string filename)
        {
            var settingFileName = EnqueueRequestDTO.AddImportExtension(filename);
            var settingFilePath = Path.Combine(folder, settingFileName);
            var json = JSONHelper.Serialize(src);
            File.WriteAllText(settingFilePath,json);
        }


        // used for tracing, and for processfile that needs all the parameters as namevalue parameters
        // need to add the rest in!
        public NameValueCollection ToNameValueCollection()
        {
            var nv = new NameValueCollection();
            nv.Add("user_id", UserId ?? string.Empty);
            nv.Add("oauth_consumer_key", OAuthConsumerKey ?? string.Empty);
            nv.Add("resource_id", Cartridge.ResourceId ?? string.Empty);
            nv.Add("resource_title",Cartridge.ResourceTitle ?? string.Empty);
            nv.Add("resource_file",Cartridge.ResourceFile);
            nv.Add("resource_authorise_type", Cartridge.ResourceAuthoriseType.ToString());
            return nv;
        }


    }
}
