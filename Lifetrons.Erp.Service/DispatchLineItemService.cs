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

namespace Lifetrons.Erp.Service
{
    public interface IDispatchLineItemService
    {
        IEnumerable<DispatchLineItem> DispatchStatus(string orderLineItemId);
        IEnumerable<DispatchLineItem> SelectLineItems(string dispatchId, string userId, string orgId);
        Task<IEnumerable<DispatchLineItem>> SelectAsyncLineItems(string dispatchId, string userId, string orgId);
        Task<DispatchLineItem> FindAsync(string id, string userId, string orgId);
        DispatchLineItem Find(string id, string userId, string orgId);
        Task<DispatchLineItem> QRFindAsync(string id);
        DispatchLineItem Create(DispatchLineItem param, string userId, string orgId);
        void Update(DispatchLineItem param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<DispatchLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }

    public class DispatchLineItemService : Service<DispatchLineItem>, IDispatchLineItemService
    {
        private readonly IRepositoryAsync<DispatchLineItem> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;

        [Dependency]
        public IDispatchService DispatchService { get; set; }

        public DispatchLineItemService(IRepositoryAsync<DispatchLineItem> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<DispatchLineItem> DispatchStatus(string orderLineItemId)
        {
            if (string.IsNullOrEmpty(orderLineItemId)) throw new ApplicationException("Parameters cannot be null or empty");
            Guid orderLineItemIdGuid = orderLineItemId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrderLineItemId == orderLineItemIdGuid)
               .Include(p => p.Order)
               .Include(p => p.OrderLineItem)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .Select();

            return enumerable;
        }

        public IEnumerable<DispatchLineItem> SelectLineItems(string dispatchId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(dispatchId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = dispatchId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.DispatchId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Order)
               .Include(p => p.OrderLineItem)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<DispatchLineItem>> SelectAsyncLineItems(string dispatchId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(dispatchId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = dispatchId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.DispatchId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Order)
               .Include(p => p.OrderLineItem)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<DispatchLineItem> FindAsync(string id, string userId, string orgId)
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

        public async Task<DispatchLineItem> QRFindAsync(string id)
        {

            var record = await _repository.FindAsync(new object[] { id.ToSysGuid() });

            return record;
        }

        public DispatchLineItem Find(string id, string userId, string orgId)
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

        public DispatchLineItem Create(DispatchLineItem param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(DispatchLineItem param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
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

        public IEnumerable<DispatchLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Quantity.ToString()))
                    .OrderBy(q => q
                        .OrderBy(c => c.DispatchId)
                        .ThenBy(c => c.Serial)
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
