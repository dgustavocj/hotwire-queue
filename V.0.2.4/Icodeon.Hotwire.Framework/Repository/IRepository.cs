namespace Icodeon.Hotwire.Framework.Repositories
{
    public interface IRepository
    {
        void Create();
        void Delete();
        bool Exists();
    }
}