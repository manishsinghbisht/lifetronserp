using System;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Models;
using Lifetrons.Erp.Repository.Repositories;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class JobProductReceiptService : Service<JobProductReceipt>, IJobProductReceiptService
    {
        private readonly IRepositoryAsync<JobProductReceipt> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public JobProductReceiptService(IRepositoryAsync<JobProductReceipt> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            :base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<JobProductReceipt> SelectLineItems(string jobReceiptId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobReceiptId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = jobReceiptId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobReceiptId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser =  _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable; 
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobProductReceipt>> SelectAsyncLineItems(string jobReceiptId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobReceiptId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = jobReceiptId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobReceiptId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobProductReceipt>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            decimal listIdGuid = Convert.ToDecimal(jobNo);
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == listIdGuid)
                                .OrderBy(q => q
                        .OrderBy(c => c.JobReceiptHead.Date)
                        .ThenBy(c => c.JobNo))
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .Include(p => p.JobReceiptHead)
               .Include(p => p.JobReceiptHead.Employee)
               .Include(p => p.JobReceiptHead.Process)
               .Include(p => p.JobReceiptHead.Process1)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<JobProductReceipt> FindAsync(string id, string userId, string orgId)
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

        public JobProductReceipt Find(string id, string userId, string orgId)
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

        public decimal TotalQuantityReceipt(string jobNo, string receiptByProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(receiptByProcessId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid receiptByProcessIdGuid = receiptByProcessId.ToSysGuid();
            decimal jobNoDecimal = Convert.ToDecimal(jobNo);

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal & p.JobReceiptHead.ReceiptByProcessId == receiptByProcessIdGuid)
               .Select();

            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public decimal TotalQuantityReceipt(string jobNo, string receiptByProcessId, string receiptFromProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(receiptByProcessId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid receiptByProcessIdGuid = receiptByProcessId.ToSysGuid();
            Guid receiptFromProcessIdGuid = receiptFromProcessId.ToSysGuid();
            decimal jobNoDecimal = Convert.ToDecimal(jobNo);

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal & p.JobReceiptHead.ReceiptByProcessId == receiptByProcessIdGuid & p.JobReceiptHead.ReceiptFromProcessId == receiptFromProcessIdGuid)
               .Select();

            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public IEnumerable<JobQuantityTotals> ProcesswiseQuantitiesGroupByJobs(DateTime startDateTime, DateTime endDateTime, string receiptByProcessId, string receiptFromProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(receiptByProcessId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid receiptByProcessIdGuid = receiptByProcessId.ToSysGuid();
            Guid receiptFromProcessIdGuid = receiptFromProcessId.ToSysGuid();

            var enumerable = _repository.GetProcesswiseQuantitiesGroupByJobs(startDateTime, endDateTime, receiptByProcessIdGuid,
                receiptFromProcessIdGuid, orgIdGuid);

            return enumerable;
        }

        public decimal AssemblyLoadIn(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid assemblyProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid
                & p.JobReceiptHead.Date >= startDateTime
                & p.JobReceiptHead.ReceiptByProcessId == assemblyProcessId)
                .Select();
            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public IEnumerable<JobQuantityTotals> AssemblyLoadInJobwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            
            var enumerable = _repository.AssemblyLoadInJobwise(startDateTime, orgIdGuid).OrderBy(p => p.JobNo);

            return enumerable;
        }


        public JobProductReceipt Create(JobProductReceipt param, string userId, string orgId)
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

        private void Validate(JobProductReceipt param, AspNetUser applicationUser)
        {

        }

        public void Update(JobProductReceipt param, string userId, string orgId)
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

        public IEnumerable<JobProductReceipt> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
