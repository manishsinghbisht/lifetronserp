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
using Microsoft.Practices.ObjectBuilder2;

namespace Lifetrons.Erp.Service
{
    public class JobReceiptHeadService : Service<JobReceiptHead>, IJobReceiptHeadService
    {
        [Dependency]
        public Repository<JobProductReceipt> LineItemRepositoryJobProducts { get; set; }

        [Dependency]
        public Repository<JobItemReceipt> LineItemRepositoryJobItems { get; set; }

         [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }

        private readonly IRepositoryAsync<JobReceiptHead> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public JobReceiptHeadService(IRepositoryAsync<JobReceiptHead> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public IEnumerable<JobReceiptHead> Select(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid quoteIdGuid = id.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == quoteIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.Process) //ReceiptBy
              .Include(p => p.Process1) //ReceiptFrom
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobReceiptHead>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.Process) //ReceiptBy
              .Include(p => p.Process1) //ReceiptFrom
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobReceiptHead>> SelectAsyncByDate(DateTime startDate, DateTime endDate, string userId, string orgId)
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

        public async Task<IEnumerable<JobReceiptHead>> SelectAsyncReceiptFromProcessIdByDate(DateTime startDate, DateTime endDate, string receiptFromProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid receiptFromProcessIdGuid = receiptFromProcessId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.Date >= startDate & p.Date <= endDate) & p.ReceiptFromProcessId == receiptFromProcessIdGuid)
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

        public async Task<IEnumerable<JobReceiptHead>> SelectAsyncReceiptByProcessIdByDate(DateTime startDate, DateTime endDate, string receiptByProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid receiptByProcessIdGuid = receiptByProcessId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.Date >= startDate & p.Date <= endDate) & p.ReceiptByProcessId == receiptByProcessIdGuid)
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

        public JobReceiptHead Find(string id, string userId, string orgId)
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

        public async Task<JobReceiptHead> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public JobReceiptHead Create(JobReceiptHead param, string userId, string orgId)
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

        private void Validate(JobReceiptHead param, AspNetUser applicationUser)
        {
            if (param.ReceiptByProcessId == Guid.Empty || param.ReceiptFromProcessId == Guid.Empty)
                throw new ApplicationException("Empty process ids");

            if (param.ReceiptByProcessId == param.ReceiptFromProcessId)
                throw new ApplicationException("Receipt by and receipt from processes cannot be same.");
        }

        public void Update(JobReceiptHead param, string userId, string orgId)
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

        public new void Delete(JobReceiptHead model)
        {
            //// Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                IRepository<JobProductReceipt> lineItemRepositoryJobProducts = LineItemRepositoryJobProducts;
                IRepository<JobItemReceipt> lineItemRepositoryjobItems = LineItemRepositoryJobItems;

                //Delete details records
                var lineProducts = lineItemRepositoryJobProducts.Query(p => p.JobReceiptId == model.Id).Select();
                lineProducts.ForEach(lineItemRepositoryJobProducts.Delete);

                var lineItems = lineItemRepositoryjobItems.Query(p => p.JobReceiptId == model.Id).Select();
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

        public IEnumerable<JobReceiptHead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Name) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Employee)
                        .ThenBy(c => c.Process1)
                        .ThenBy(c => c.Process))
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
