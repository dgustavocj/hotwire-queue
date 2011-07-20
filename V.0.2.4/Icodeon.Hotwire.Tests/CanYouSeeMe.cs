using System;
using Icodeon.Hotwire.Framework.Configuration;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.Internal
{
    [TestFixture]
    public class CanYouSeeMe
    {

        [Test]
        public void YabbaDabbaDooo()
        {
#if DEBUG
            throw new Exception("Can you this? Apparently this is a debug configuration!");
#else
            throw new Exception("Nope... we are NOT in debug mode...most likely RELEASE, will check further later.");
#endif
        }
    }
}