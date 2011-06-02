namespace Icodeon.Hotwire.Contracts
{

    //ADH: check to see if this class can be removed / deleted.
    public interface ITestHelperProvider 
    {
        /// <summary>
        /// Deletes the resource and phyiscally removes any records from the db, with cascade delete or similar.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <remarks>HardDeleteResource should be idempotent, and should only throw an exception if resource exists and cannot be deleted for any reason. If resourceId does not exit then should do nothing.</remarks>
        void HardDeleteResource(string resourceId);
    }
}
