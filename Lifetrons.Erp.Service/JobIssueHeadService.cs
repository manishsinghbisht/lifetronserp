using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using Microsoft.Ajax.Utilities;

namespace Lifetrons.Erp.Service
{
    public class JobIssueHeadService : Service<JobIssueHead>, IJobIssueHeadService
    {
        [Dependency]
        public Repository<JobProductIssue> LineItemRepositoryJobProducts { get; set; }

        [Dependency]
        public Repository<JobItemIssue> LineItemRepositoryJobItems { get; set; }

        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }

        private readonly IRepositoryAsync<JobIssueHead> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public JobIssueHeadService(IRepositoryAsync<JobIssueHead> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public IEnumerable<JobIssueHead> Select(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid quoteIdGuid = id.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == quoteIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
               .Include(p => p.Process) //IssuedBy
              .Include(p => p.Process1) //IssuedTo
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobIssueHead>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.Process) //IssuedBy
              .Include(p => p.Process1) //IssuedTo
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobIssueHead>> SelectAsyncByDate(DateTime startDate, DateTime endDate, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.Date >= startDate & p.Date <= endDate))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.Process) //IssuedBy
              .Include(p => p.Process1) //IssuedTo
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobIssueHead>> SelectAsyncIssuedToProcessIdByDate(DateTime startDate, DateTime endDate, string issuedToProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedToProcessIdGuid = issuedToProcessId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.Date >= startDate & p.Date <= endDate) & p.IssuedToProcessId == issuedToProcessIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.Process) //IssuedBy
              .Include(p => p.Process1) //IssuedTo
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobIssueHead>> SelectAsyncIssuedByProcessByDate(DateTime startDate, DateTime endDate, string issuedByProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedByProcessIdGuid = issuedByProcessId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.Date >= startDate & p.Date <= endDate) & p.IssuedByProcessId == issuedByProcessIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.Process) //IssuedBy
              .Include(p => p.Process1) //IssuedTo
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

       public JobIssueHead Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var record = _repository.Find(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<JobIssueHead> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public JobIssueHead Create(JobIssueHead param, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business validation
                Validate(param, applicationUser);
                //Create
                _repository.Insert(param);

                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        private void Validate(JobIssueHead param, AspNetUser applicationUser)
        {
            if (param.IssuedByProcessId == Guid.Empty || param.IssuedToProcessId == Guid.Empty)
                throw new ApplicationException("Empty process ids");

            if (param.IssuedByProcessId == param.IssuedToProcessId)
                throw new ApplicationException("Issued by and issued to processes cannot be same.");

        }

        public void Update(JobIssueHead param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business validation
                Validate(param, applicationUser);

                //Update
                _repository.Update(param);
            }
            else
            {
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            }
        }

        public new void Delete(JobIssueHead model)
        {
            //// Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                IRepository<JobProductIssue> lineItemRepositoryJobProducts = LineItemRepositoryJobProducts;
                IRepository<JobItemIssue> lineItemRepositoryjobItems = LineItemRepositoryJobItems;

                //Delete details records
                var lineProducts = lineItemRepositoryJobProducts.Query(p => p.JobIssueId == model.Id).Select();
                lineProducts.ForEach(lineItemRepositoryJobProducts.Delete);

                var lineItems = lineItemRepositoryjobItems.Query(p => p.JobIssueId == model.Id).Select();
                lineItems.ForEach(lineItemRepositoryjobItems.Delete);

                //Delete master records 
                _repository.Delete(model);

                UnitOfWork.SaveChanges();

                //// Commit Transaction
                UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                //// Rollback transaction
                UnitOfWork.Rollback();

                throw;
            }
        }

        public IEnumerable<JobIssueHead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Name) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Employee)
                        .ThenBy(c => c.Process)
                        .ThenBy(c => c.Process1))
                    .SelectPage(pageNumber, pageSize, out totalRecords);

                return records;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        public void Dispose()
        {
        }

    }
}
