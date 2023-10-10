using System;
using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using Lifetrons.Erp.Repository.Repositories;

namespace Lifetrons.Erp.Service
{
    public class StockProductIssueService : Service<StockProductIssue>, IStockProductIssueService
    {
        private readonly IRepositoryAsync<StockProductIssue> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;

       
        public StockProductIssueService(IRepositoryAsync<StockProductIssue> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            :base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<StockProductIssue> SelectLineItems(string stockIssueId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(stockIssueId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = stockIssueId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StockIssueId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser =  _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable; 
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<StockProductIssue>> SelectAsyncLineItems(string stockIssueId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(stockIssueId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = stockIssueId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StockIssueId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<StockProductIssue>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            decimal listIdGuid = Convert.ToDecimal(jobNo);
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == listIdGuid)
                .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Product)
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

        public IEnumerable<StockProductQuantityTotals> CurrentIssueStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.IssueStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockProductQuantityTotals> FGCurrentIssueStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.FGIssueStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockProductQuantityTotals> RawCurrentIssueStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.RawIssueStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockProductQuantityTotals> ScrapCurrentIssueStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.ScrapIssueStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }

        public IEnumerable<StockProductQuantityTotals> ProductIssueStatus(DateTime startDateTime, string productId, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid productIdGuid = productId.ToSysGuid();

            var enumerable = _repository.ProductIssueStatus(Convert.ToDateTime(startDateTime), productIdGuid, orgIdGuid);

            return enumerable;
        }


        public async Task<StockProductIssue> FindAsync(string id, string userId, string orgId)
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

        public StockProductIssue Find(string id, string userId, string orgId)
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

        public StockProductIssue Create(StockProductIssue param, string userId, string orgId)
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

        private void Validate(StockProductIssue param, AspNetUser applicationUser)
        {
            //Check Item
            if (param.ProductId == null) throw new ApplicationException("Product cannot be null");

            if (param.JobNo == null && param.CaseNo == null && param.TaskNo == null) throw new ApplicationException("Either Job No or Case No or Task No is required.");

            //Check negative quanity
            //Check start and end dates
            if (param.Quantity < 1) throw new ApplicationException("Quantity cannot be less than 1.");
        }

        public void Update(StockProductIssue param, string userId, string orgId)
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

        public IEnumerable<StockProductIssue> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Quantity.ToString()))
                    .OrderBy(q => q
                        .OrderBy(c => c.Serial)
                        .ThenBy(c => c.ProductId))
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
