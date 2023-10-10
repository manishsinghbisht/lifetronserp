using System;
using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Service
{
    public class OrderLineItemService : Service<OrderLineItem>, IOrderLineItemService
    {
        [Dependency]
        public IOrderService OrderService { get; set; }

        [Dependency]
        public IProductService ProductService { get; set; }

        private readonly IRepositoryAsync<OrderLineItem> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public OrderLineItemService(IRepositoryAsync<OrderLineItem> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<OrderLineItem> SelectLineItems(string orderId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = orderId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OrderId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Order)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .OrderBy(q => q.OrderBy(c => c.Serial).ThenBy(c => c.Product.Name))
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<OrderLineItem>> SelectAsyncLineItems(string orderId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = orderId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OrderId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Order)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .OrderBy(q => q.OrderBy(c => c.Serial).ThenBy(c => c.Product.Name))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<OrderLineItem> SelectSingleAsync(string orderLineItemId, string orderId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(orderLineItemId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;
            Guid orgIdGuid = orgId.ToSysGuid();
            Guid orderLineItemIdGuid = orderLineItemId.ToSysGuid();
            Guid listIdGuid = orderId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == orderLineItemIdGuid & p.OrderId == listIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Order)
              .Include(p => p.PriceBook)
              .Include(p => p.Product)
              .SelectAsync();

            if (!enumerable.Any())
            {
                throw new Exception("Order item not found");
            }
            return enumerable.First();
        }

        public OrderLineItem SelectSingle(string orderLineItemId, string orderId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(orderLineItemId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;
            Guid orgIdGuid = orgId.ToSysGuid();
            Guid orderLineItemIdGuid = orderLineItemId.ToSysGuid();
            Guid listIdGuid = orderId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == orderLineItemIdGuid & p.OrderId == listIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Order)
              .Include(p => p.PriceBook)
              .Include(p => p.Product)
              .Select();

            if (!enumerable.Any())
            {
                throw new Exception("Order item not found");
            }
            return enumerable.First();
        }

        public async Task<OrderLineItem> SelectSingleAsync(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;
            Guid orgIdGuid = orgId.ToSysGuid();
            Decimal jobNoDecimal = Convert.ToDecimal(jobNo);
            
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Order)
              .Include(p => p.PriceBook)
              .Include(p => p.Product)
              .SelectAsync();

            if (!enumerable.Any())
            {
                throw new Exception("Order item not found");
            }
            return enumerable.First();
        }

        public OrderLineItem SelectSingle(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;
            Guid orgIdGuid = orgId.ToSysGuid();
            Decimal jobNoDecimal = Convert.ToDecimal(jobNo);

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Order)
              .Include(p => p.PriceBook)
              .Include(p => p.Product)
              .Include(p => p.Product.Operations)
              .Select();

            if (!enumerable.Any())
            {
                throw new Exception("Order item not found");
            }
            return enumerable.First();
        }
        public OrderLineItem SelectSingle(string jobNo)
        {
            if (string.IsNullOrEmpty(jobNo)) return null;
            Decimal jobNoDecimal = Convert.ToDecimal(jobNo);

            var enumerable = _repository.Query(p => p.JobNo == jobNoDecimal)
              .Include(p => p.Order)
              .Include(p => p.PriceBook)
              .Include(p => p.Product)
              .Include(p => p.Product.Operations)
              .Select();

            if (!enumerable.Any())
            {
                throw new Exception("Order item not found");
            }

            return enumerable.First();
        }
        public async Task<OrderLineItem> FindAsync(string orderId, string priceBookId, string productId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(new object[] { orderId.ToSysGuid(), priceBookId.ToSysGuid(), productId.ToSysGuid() });

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record == null) return null;
            if (record.OrgId == applicationUser.OrgId)
            {
                record.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                record.Product = await ProductService.FindAsync(orgId, userId, orgId);
                record.Order = await OrderService.FindAsync(orgId, userId, orgId);
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        
        public OrderLineItem Create(OrderLineItem param, string userId, string orgId)
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

        private void Validate(OrderLineItem param, AspNetUser applicationUser)
        {

        }

        public void Update(OrderLineItem param, string userId, string orgId)
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

        public IEnumerable<OrderLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Product.Code))
                    .OrderBy(q => q
                        .OrderBy(c => c.OrderId)
                        .ThenBy(c => c.PriceBook)
                        .ThenBy(c => c.Product))
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
