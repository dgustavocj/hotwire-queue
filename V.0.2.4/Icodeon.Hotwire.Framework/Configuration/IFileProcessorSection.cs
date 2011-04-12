namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface IFileProcessorSection : IAssemblyProvider
    {
        int MaxFileProcessorWorkers { get; }
    }
}