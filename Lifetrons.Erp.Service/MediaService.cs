using Lifetrons.Erp.Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;
using Task = System.Threading.Tasks.Task;


namespace Lifetrons.Erp.Service
{
    public class MediaService : Service<Media>, IMediaService
    {
        private readonly IRepositoryAsync<Media> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;

        public MediaService(IRepositoryAsync<Media> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }


        public async Task<IEnumerable<Media>> FindAsyncByParent(string parentType, string parentId)
        {
            Guid parentIdGuid = parentId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.ParentId == parentIdGuid & p.ParentType == parentType)
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.Tags))
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<Media> FindByParent(string parentType, string parentId)
        {
            Guid parentIdGuid = parentId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.ParentId == parentIdGuid & p.ParentType == parentType)
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.Tags))
              .Select();

            return enumerable;
        }

        public async Task<IEnumerable<Media>> FindAsyncByParentId(string parentId)
        {
            Guid parentIdGuid = parentId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.ParentId == parentIdGuid)
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.Tags))
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<Media> FindByParentId(string parentId)
        {
            Guid parentIdGuid = parentId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.ParentId == parentIdGuid)
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.Tags))
              .Select();

            return enumerable;
        }

        public async Task<Media> FindAsyncProfilePic(string userId)
        {
            var enumerable = await _repository.Query(p => p.Active & p.CreatedBy == userId & p.ParentType == "Profile")
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
              .SelectAsync();

            return enumerable.FirstOrDefault<Media>();
        }

        public async Task<string> GetAsyncProfilePicPath(string userId)
        {
            var enumerable = await _repository.Query(p => p.Active & p.CreatedBy == userId & p.ParentType == "Profile")
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
              .SelectAsync();

            return enumerable.FirstOrDefault<Media>().MediaPath;
        }

        public string GetProfilePicPath(string userId)
        {
            var enumerable = _repository.Query(p => p.Active & p.CreatedBy == userId & p.ParentType == "Profile")
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
              .Select();

            if (enumerable.Count() > 0)
            {
                return enumerable.FirstOrDefault<Media>().MediaPath;
            }

            return null;
        }

        public async Task<IEnumerable<Media>> FindAsyncActiveByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var enumerable = await _repository.Query(p => p.Active & p.CreatedBy == userId)
                .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
                .SelectAsync();

            return enumerable;
        }

        public async Task<IEnumerable<Media>> FindAsyncAllByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var enumerable = await _repository.Query(p => p.CreatedBy == userId)
                .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
                .SelectAsync();

            return enumerable;
        }

        public Media Find(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var Media = _repository.Find(id.ToSysGuid());
            if (Media == null) return null;

            return Media;
        }

        public async Task<Media> FindAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var Media = await _repository.FindAsync(id.ToSysGuid());
            if (Media == null) return null;

            return Media;
        }

        public Media Create(Media param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            _repository.Insert(param);
            return param;

        }

        public void Update(Media param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            //Update
            _repository.Update(param);

        }

        public void Delete(string id)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public IEnumerable<Media> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var records = _repository
                .Query(q => !string.IsNullOrEmpty(q.ParentType) & q.OrgId.Equals(orgIdGuid))
                .OrderBy(q => q
                    .OrderBy(c => c.ParentType)
                    .ThenBy(c => c.Tags))
                .SelectPage(pageNumber, pageSize, out totalRecords);

            return records;

        }

        public void Dispose()
        {
        }

    }
}
