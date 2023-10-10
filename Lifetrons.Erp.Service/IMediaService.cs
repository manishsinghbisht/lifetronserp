namespace Lifetrons.Erp.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lifetrons.Erp.Data;

    public interface IMediaService
    {
        Task<IEnumerable<Media>> FindAsyncByParent(string parentType, string parentId);
        IEnumerable<Media> FindByParent(string parentType, string parentId);
        Task<IEnumerable<Media>> FindAsyncByParentId(string parentId);
        IEnumerable<Media> FindByParentId(string parentId);
        Task<Media> FindAsyncProfilePic(string userId);
        Task<string> GetAsyncProfilePicPath(string userId);
        string GetProfilePicPath(string userId);
        Task<IEnumerable<Media>> FindAsyncActiveByUserId(string userId);
        Task<IEnumerable<Media>> FindAsyncAllByUserId(string userId);
        Media Find(string id);
        Task<Media> FindAsync(string id);
        Media Create(Media param, string userId, string orgId);
        void Update(Media param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<Media> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}