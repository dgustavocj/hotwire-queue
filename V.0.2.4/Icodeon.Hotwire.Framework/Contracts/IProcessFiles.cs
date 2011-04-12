using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IProcessFiles
    {
        IEnumerable<string> DownloadingFilePaths { get; }
        IEnumerable<string> DownloadQueueFilePaths { get; }
        IEnumerable<string> ProcessQueueFilePaths { get;  }
        IEnumerable<string> ProcessedFilePaths { get; }
        IEnumerable<string> ProcessingFilePaths { get; }
        IEnumerable<string> ProcessErrorFilePaths { get; }
    }
}
