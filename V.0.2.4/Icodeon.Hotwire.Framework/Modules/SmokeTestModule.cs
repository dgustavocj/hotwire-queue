using System.Collections.Generic;
using System.Net;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class SmokeTestModule : ModuleBase
    {
        public const string SmokeTestSectionName = "smokeTests";

        protected override string ConfigurationSectionName
        {
            get { return SmokeTestSectionName; }
        }

        public override object ProcessRequest(ParsedContext context)
        {
            // we should never get here, because we only want to use the default "inherited" echo and "version" actions
            throw new HttpModuleException(HttpStatusCode.BadRequest, context.ModuleConfig.Action + " is not a supported action.");
        }

        public const string SlashEncodingToken = "~";



        public override IEnumerable<string> ActionNames
        {
            get { return new string[] { }; }
        }


    } 
} 