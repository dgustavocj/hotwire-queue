using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IHotwireFileProcessorFoldersPaths 
    {
        string DownloadingFolderPath { get; }
        string ProcessQueueFolderPath { get; }
        string ProcessedFolderPath { get; }
        string ProcessingFolderPath { get; }
        string ProcessErrorFolderPath { get; }
        string SolutionFolderMarkerFile { get; }
    }
}
