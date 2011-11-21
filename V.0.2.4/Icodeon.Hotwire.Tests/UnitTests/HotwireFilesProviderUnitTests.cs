using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class HotwireFilesProviderUnitTests
    {

        public void Test()
        {

            // what are we trying to test, or prove
            // I saw the code, and was worried that a lock was being held around the object, even after it was returned,
            // so the lock possibly lasts as long as the returned object holds the lock.

            // Q: What problem will/could this cause?
            // Q: How can we prove that the problem exists, and that the changed code fixes the problem, and does not in fact introduce more problems?

            // NB! come back to this, after all the tests are green again! (small steps)

        }

        
        // this is what the code SHOULD look like...

        //        lock(locker)
        //{
        //    if (_hotwireFilesProvider==null)
        //    {
        //        _logger.Trace("HotwireFilesProvider GetFilesProviderInstance(...)");
        //        _logger.Trace(_hotwireFilesProvider == null ? "creating new instance of HotwireFilesProvider" : "reading  filesProvider config");
        //        var foldersSection = FoldersSection.ReadConfig();
        //        _hotwireFilesProvider = new HotwireFilesProvider(foldersSection, refreshFiles);
        //    }
        //}
        //return _hotwireFilesProvider;

    }
}
