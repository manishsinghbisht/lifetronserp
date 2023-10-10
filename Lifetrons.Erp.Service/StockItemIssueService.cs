using System;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Repositories;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class StockItemIssueService : Service<StockItemIssue>, IStockItemIssueService
    {
        private readonly IRepositoryAsync<StockItemIssue> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;

       
        public StockItemIssueService(IRepositoryAsync<StockItemIssue> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            :base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<StockItemIssue> SelectLineItems(string stockIssueId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(stockIssueId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = stockIssueId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StockIssueId == listIdGuid)
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

        public async Task<IEnumerable<StockItemIssue>> SelectAsyncLineItems(string stockIssueId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(stockIssueId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = stockIssueId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StockIssueId == listIdGuid)
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

        public async Task<IEnumerable<StockItemIssue>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId)
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
               .Include(p => p.StockIssueHead)
               .Include(p => p.StockIssueHead.Employee)
               .Include(p => p.StockIssueHead.Process)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<StockItemQuantityTotals> CurrentIssueStatusItemwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.IssueStatusItemwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockItemQuantityTotals> FGCurrentIssueStatusItemwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.FGIssueStatusItemwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockItemQuantityTotals> RawCurrentIssueStatusItemwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.RawIssueStatusItemwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockItemQuantityTotals> ScrapCurrentIssueStatusItemwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.ScrapIssueStatusItemwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }

        public IEnumerable<StockItemQuantityTotals> ItemIssueStatus(DateTime startDateTime, string itemId, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid itemIdGuid = itemId.ToSysGuid();

            var enumerable = _repository.ItemIssueStatus(Convert.ToDateTime(startDateTime), itemIdGuid, orgIdGuid);

            return enumerable;
        }

        public async Task<StockItemIssue> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(new object[] { id.ToSysGuid() });
            
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record == null) return null;
            if (record.OrgId == applicationUser.OrgId)
            {
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public StockItemIssue Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = _repository.Find(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (record == null) return null;

            if (record.OrgId == applicationUser.OrgId)
            {
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public StockItemIssue Create(StockItemIssue param, string userId, string orgId)
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

        private void Validate(StockItemIssue param, AspNetUser applicationUser)
        {
            //Check Item
            if (param.ItemId == null) throw new ApplicationException("Item cannot be null");

            if (param.JobNo == null && param.CaseNo == null && param.TaskNo == null) throw new ApplicationException("Either Job No or Case No or Task No is required.");

            //Check negative quanity
            //Check start and end dates
            if (param.Quantity < 1) throw new ApplicationException("Quantity cannot be less than 1.");
        }

        public void Update(StockItemIssue param, string userId, string orgId)
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

        public IEnumerable<StockItemIssue> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Quantity.ToString()))
                    .OrderBy(q => q
                        .OrderBy(c => c.Serial)
                        .ThenBy(c => c.ItemId))
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
