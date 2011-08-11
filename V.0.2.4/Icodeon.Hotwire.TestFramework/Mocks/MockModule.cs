using System;
using System.Collections.Generic;
using System.IO;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockModule : ModuleBase
    {
        protected override string ConfigurationSectionName
        {
            get { throw new Exception(); }
        }

        public override IEnumerable<string> ActionNames
        {
            get { return new[] { ActionRun, ActionWalk, ActionQuote }; }
        }

        public const string ActionRun = "run";
        public const string ActionWalk = "walk";
        public const string ActionQuote = "quote";
        public const string ActionIOException = "ioexception";

        public override object ProcessRequest(ParsedContext context)
        {
            switch (context.ModuleConfig.Action)
            {
                case ActionRun: return "Never run with scissors";
                case ActionWalk: return "better to walk";
                case ActionIOException : throw new IOException();
            }
            return null;
        }
    }
}