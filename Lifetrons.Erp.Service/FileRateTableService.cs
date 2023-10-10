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
    public class FileRateTableService : Service<FileRateTable>, IFileRateTableService
    {
        private readonly IRepositoryAsync<FileRateTable> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;

        public FileRateTableService(IRepositoryAsync<FileRateTable> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public async Task<IEnumerable<FileRateTable>> GetAsync(string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .OrderBy(q => q.OrderByDescending(c => c.TemplateName).ThenBy(c => c.ContactId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .SelectAsync();

            return enumerable;
        }

        public async Task<IEnumerable<FileRateTable>> FindAsyncByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime)
        {
            Guid accountIdGuid = accountId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.AccountId == accountIdGuid & p.FileType == fileType & (p.StartDate <= startDateTime && p.EndDate >= endDateTime))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<FileRateTable> FindByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime)
        {
            Guid accountIdGuid = accountId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.AccountId == accountIdGuid & p.FileType == fileType & (p.StartDate <= startDateTime && p.EndDate >= endDateTime))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
              .Select();

            return enumerable;
        }

        public IEnumerable<FileRateTable> FindByContact(string fileType, string contactId, DateTime startDateTime, DateTime endDateTime)
        {
            Guid contactIdGuid = contactId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.ContactId == contactIdGuid & p.FileType == fileType & (p.StartDate <= startDateTime && p.EndDate >= endDateTime))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Select();

            return enumerable;
        }

        public FileRateTable Find(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var File = _repository.Find(id.ToSysGuid());
            if (File == null) return null;

            return File;
        }

        public async Task<FileRateTable> FindAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var File = await _repository.FindAsync(id.ToSysGuid());
            if (File == null) return null;

            return File;
        }

        public FileRateTable Create(FileRateTable param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            _repository.Insert(param);
            return param;

        }

        public void Update(FileRateTable param, string userId, string orgId)
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

        public IEnumerable<FileRateTable> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var records = _repository
                .Query(q => !string.IsNullOrEmpty(q.FileType) & q.OrgId.Equals(orgIdGuid))
                .OrderBy(q => q
                    .OrderBy(c => c.FileType)
                    .ThenBy(c => c.Account.CreatedDate))
                .SelectPage(pageNumber, pageSize, out totalRecords);

            return records;

        }

        public void Dispose()
        {
        }

    }
}
