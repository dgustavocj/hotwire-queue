using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{

    /// <summary>
    /// These are relative paths, all relative to a single solution root folder
    /// </summary>
    public interface IHotwireFileProcessorRelativeFolders
    {
        string TestDataFolder { get; }
        string DownloadErrorFolder { get; }
        string DownloadingFolder { get;  }
        string DownloadQueueFolder { get; }
        string ProcessQueueFolder { get; }
        string ProcessedFolder { get; }
        string ProcessingFolder { get; }
        string ProcessErrorFolder { get; }
        string SolutionFolderMarkerFile { get; }
    }
}
