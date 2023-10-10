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
    public class JobProductIssueService : Service<JobProductIssue>, IJobProductIssueService
    {
        private readonly IRepositoryAsync<JobProductIssue> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public JobProductIssueService(IRepositoryAsync<JobProductIssue> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<JobProductIssue> SelectLineItems(string jobIssueId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobIssueId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = jobIssueId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobIssueId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<JobProductIssue>> SelectAsyncLineItems(string jobIssueId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobIssueId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = jobIssueId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobIssueId == listIdGuid)
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

        public async Task<IEnumerable<JobProductIssue>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            decimal jobNoDecimal = Convert.ToDecimal(jobNo);
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal)
                .OrderBy(q => q
                        .OrderBy(c => c.JobIssueHead.Date)
                        .ThenBy(c => c.JobNo))
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .Include(p => p.JobIssueHead)
               .Include(p => p.JobIssueHead.Employee)
               .Include(p => p.JobIssueHead.Process)
               .Include(p => p.JobIssueHead.Process1)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public decimal TotalQuantityIssued(string jobNo, string issuedToProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(issuedToProcessId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedToProcessIdGuid = issuedToProcessId.ToSysGuid();
            decimal jobNoDecimal = Convert.ToDecimal(jobNo);

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal & p.JobIssueHead.IssuedToProcessId == issuedToProcessIdGuid)
               .Select();

            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public decimal TotalQuantityIssued(string jobNo, string issuedByProcessId, string issuedToProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(issuedToProcessId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedToProcessIdGuid = issuedToProcessId.ToSysGuid();
            Guid issuedByProcessIdGuid = issuedByProcessId.ToSysGuid();
            decimal jobNoDecimal = Convert.ToDecimal(jobNo);

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal & p.JobIssueHead.IssuedToProcessId == issuedToProcessIdGuid & p.JobIssueHead.IssuedByProcessId == issuedByProcessIdGuid)
               .Select();

            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }
        public decimal ProcessLoadIn(DateTime startDateTime, string issuedToProcessId, string orgId)
        {
            if (string.IsNullOrEmpty(issuedToProcessId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedToProcessIdGuid = issuedToProcessId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid
                & p.JobIssueHead.Date >= startDateTime
                & p.JobIssueHead.IssuedToProcessId == issuedToProcessIdGuid)
                .Select();

            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public decimal ProcessLoadOut(DateTime startDateTime, string issuedByProcessId, string orgId)
        {
            if (string.IsNullOrEmpty(issuedByProcessId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedByProcessIdGuid = issuedByProcessId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid
                & p.JobIssueHead.Date >= startDateTime
                & p.JobIssueHead.IssuedByProcessId == issuedByProcessIdGuid)
                .Select();
            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public decimal AssemblyLoadOut(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedByProcessIdGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid
                & p.JobIssueHead.Date >= startDateTime
                & p.JobIssueHead.IssuedByProcessId == issuedByProcessIdGuid)
                .Select();
            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public IEnumerable<JobQuantityTotals> AssemblyLoadOutJobwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedByProcessIdGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();
            var enumerable = _repository.ProcessLoadOutJobwise(startDateTime, issuedByProcessIdGuid, orgIdGuid).OrderBy(p => p.JobNo);

            return enumerable;
        }

        public IEnumerable<JobQuantityTotals> ProcessLoadInJobwise(DateTime startDateTime, string issuedToProcessId, string orgId)
        {
            if (string.IsNullOrEmpty(issuedToProcessId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedToProcessIdGuid = issuedToProcessId.ToSysGuid();
            var enumerable = _repository.ProcessLoadInJobwise(startDateTime, issuedToProcessIdGuid, orgIdGuid).OrderBy(p => p.JobNo);

            return enumerable;
        }
        public IEnumerable<JobQuantityTotals> ProcessLoadOutJobwise(DateTime startDateTime, string issuedByProcessId, string orgId)
        {
            if (string.IsNullOrEmpty(issuedByProcessId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedByProcessIdGuid = issuedByProcessId.ToSysGuid();
            var enumerable = _repository.ProcessLoadOutJobwise(startDateTime, issuedByProcessIdGuid, orgIdGuid).OrderBy(p => p.JobNo);

            return enumerable;
        }

        public IEnumerable<JobQuantityTotals> ProcesswiseQuantitiesGroupByJobs(DateTime startDateTime, DateTime endDateTime, string issuedByProcessId, string issuedToProcessId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(issuedToProcessId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid issuedToProcessIdGuid = issuedToProcessId.ToSysGuid();
            Guid issuedByProcessIdGuid = issuedByProcessId.ToSysGuid();

            var enumerable = _repository.GetProcesswiseQuantityTotalsGroupByJobs(startDateTime, endDateTime, issuedByProcessIdGuid, issuedToProcessIdGuid,
                 orgIdGuid);

            return enumerable;
        }

        public async Task<JobProductIssue> FindAsync(string id, string userId, string orgId)
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

        public JobProductIssue Find(string id, string userId, string orgId)
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

        public JobProductIssue Create(JobProductIssue param, string userId, string orgId)
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

        private void Validate(JobProductIssue param, AspNetUser applicationUser)
        {

        }

        public void Update(JobProductIssue param, string userId, string orgId)
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

        public IEnumerable<JobProductIssue> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
