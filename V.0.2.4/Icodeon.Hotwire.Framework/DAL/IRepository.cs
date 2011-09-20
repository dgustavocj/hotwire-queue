namespace Icodeon.Hotwire.Framework.DAL
{
    public interface IRepository
    {
        void Create();
        void Delete();
        bool Exists();
        void SubmitChanges();
    }
}