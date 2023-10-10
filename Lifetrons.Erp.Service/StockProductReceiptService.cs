using System;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Lifetrons.Erp.Repository.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class StockProductReceiptService : Service<StockProductReceipt>, IStockProductReceiptService
    {
        private readonly IRepositoryAsync<StockProductReceipt> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork; 

        public StockProductReceiptService(IRepositoryAsync<StockProductReceipt> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<StockProductReceipt> SelectLineItems(string stockReceiptId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(stockReceiptId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = stockReceiptId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StockReceiptId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<StockProductReceipt>> SelectAsyncLineItems(string stockReceiptId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(stockReceiptId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = stockReceiptId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StockReceiptId == listIdGuid)
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

        public async Task<IEnumerable<StockProductReceipt>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId)
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
               .Include(p => p.StockReceiptHead)
               .Include(p => p.StockReceiptHead.Employee)
               .Include(p => p.StockReceiptHead.Process)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<StockProductQuantityTotals> CurrentReceiptStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.ReceiptStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockProductQuantityTotals> FGCurrentReceiptStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.FGReceiptStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockProductQuantityTotals> RawCurrentReceiptStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.RawReceiptStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }
        public IEnumerable<StockProductQuantityTotals> ScrapCurrentReceiptStatusProductwise(DateTime startDateTime, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.ScrapReceiptStatusProductwise(Convert.ToDateTime(startDateTime), orgIdGuid);

            return enumerable;
        }

        public IEnumerable<StockProductQuantityTotals> ProductReceiptStatus(DateTime startDateTime, string productId, string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid productIdGuid = productId.ToSysGuid();

            var enumerable = _repository.ProductReceiptStatus(Convert.ToDateTime(startDateTime), productIdGuid, orgIdGuid);

            return enumerable;
        }


        public async Task<StockProductReceipt> FindAsync(string id, string userId, string orgId)
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

        public StockProductReceipt Find(string id, string userId, string orgId)
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

        public StockProductReceipt Create(StockProductReceipt param, string userId, string orgId)
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

        private void Validate(StockProductReceipt param, AspNetUser applicationUser)
        {
            //Check Item
            if (param.ProductId == null) throw new ApplicationException("Product cannot be null");

            if (param.JobNo == null && param.CaseNo == null && param.TaskNo == null) throw new ApplicationException("Either Job No or Case No or Task No is required.");

            //Check negative quanity
            //Check start and end dates
            if (param.Quantity < 1) throw new ApplicationException("Quantity cannot be less than 1.");
        }

        public void Update(StockProductReceipt param, string userId, string orgId)
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

        public IEnumerable<StockProductReceipt> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
