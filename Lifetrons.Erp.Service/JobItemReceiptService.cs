using System;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class JobItemReceiptService : Service<JobItemReceipt>, IJobItemReceiptService
    {
        private readonly IRepositoryAsync<JobItemReceipt> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public JobItemReceiptService(IRepositoryAsync<JobItemReceipt> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            :base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<JobItemReceipt> SelectLineItems(string jobReceiptId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobReceiptId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = jobReceiptId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobReceiptId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser =  _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable; 
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobItemReceipt>> SelectAsyncLineItems(string jobReceiptId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobReceiptId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = jobReceiptId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobReceiptId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobItemReceipt>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            decimal listIdGuid = Convert.ToDecimal(jobNo);
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .Include(p => p.JobReceiptHead)
               .Include(p => p.JobReceiptHead.Employee)
               .Include(p => p.JobReceiptHead.Process)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<JobItemReceipt> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(new object[] { id.ToSysGuid() });
            
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record == null) return null;
            if (record.OrgId == applicationUser.OrgId)
            {
                record.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public JobItemReceipt Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = _repository.Find(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (record == null) return null;

            if (record.OrgId == applicationUser.OrgId)
            {
                record.Organization = _organizationService.Find(orgId, userId, orgId);
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public JobItemReceipt Create(JobItemReceipt param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

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

        private void Validate(JobItemReceipt param, AspNetUser applicationUser)
        {

        }

        public void Update(JobItemReceipt param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
           
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

        public void Delete(string id)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public IEnumerable<JobItemReceipt> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Quantity.ToString()))
                    .OrderBy(q => q
                        .OrderBy(c => c.Serial)
                        .ThenBy(c => c.JobNo)
                        .ThenBy(c => c.Quantity))
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
