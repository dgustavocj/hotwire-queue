using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockModule : ModuleBase
    {
        protected override string ConfigurationSectionName
        {
            get { return "mockModule"; }
        }

        public override IEnumerable<string> ActionNames
        {
            get { return new[] { ActionRun, ActionWalk, ActionQuote,ActionHttpModuleException }; }
        }

        public const string ActionRun = "run";
        public const string ActionWalk = "walk";
        public const string ActionQuote = "quote";
        public const string ActionHttpModuleException = "httpModuleException";

        public override object ProcessRequest(ParsedContext context)
        {
            switch (context.ModuleConfig.Action)
            {
                case ActionRun: return "Never run with scissors";
                case ActionWalk: return "better to walk";
                case ActionHttpModuleException: throw new HttpModuleException(HttpStatusCode.MovedPermanently, "(moved permanently - thrown by MockModule )");
            }
            return null;
        }
    }
}