using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Queries;
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
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Service
{
    public class OrderService : Service<Order>, IOrderService
    {
        [Dependency]
        public Repository<OrderLineItem> LineItemRepository { get; set; }

        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }

        private readonly IRepositoryAsync<Order> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public OrderService(IRepositoryAsync<Order> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<Order>> GetOrdersByAccountAsync(DateTime startDateTime, DateTime endDateTime, string accountId, string orgId)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid accountIdGuid = accountId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.AccountId == accountIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.AspNetUserReportCompletionTo)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .SelectAsync();

            return enumerable;
        }

        public async Task<IEnumerable<Order>> GetOrdersBySubAccountAsync(DateTime startDateTime, DateTime endDateTime, string subAccountId, string orgId)
        {
            if (string.IsNullOrEmpty(subAccountId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid subAccountIdGuid = subAccountId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.SubAccountId == subAccountIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.AspNetUserReportCompletionTo)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<Order> GetOrdersByUserList(DateTime startDateTime, DateTime endDateTime, string userList, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime) & userList.Contains(p.CreatedBy))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<Order> GetOrdersByOrganization(DateTime startDateTime, DateTime endDateTime, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.AspNetUserReportCompletionTo)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<Order> GetOrdersByOwner(DateTime startDateTime, DateTime endDateTime, string ownerId, string orgId)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OwnerId == ownerId & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .Select();

            return enumerable;
        }

        public IEnumerable<Order> GetOrdersByAmountAndDateRange(DateTime fromDateTime, DateTime toDateTime, decimal amount, string country)
        {
            var enumerable = _repository.Query(new OrderSalesQuery()
            {
                Amount = amount,
                Country = country,
                //FromDate = DateTime.Parse("01/01/2014"), 
                //ToDate = DateTime.Parse("12/31/2016" )
                FromDate = DateTime.Parse(fromDateTime.ToString()),
                ToDate = DateTime.Parse(toDateTime.ToString())
            })
            .Select();
            
            return enumerable;
        }

        public IEnumerable<Order> Select(string orderId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid orderIdGuid = orderId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == orderIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.AspNetUserReportCompletionTo)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Order>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OwnerId == userId)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.AspNetUserReportCompletionTo)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Order>> SelectAsyncAllOrders(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2) //Owner
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Address) //Billing
              .Include(p => p.Address1) //Shipping
              .Include(p => p.Contact) //Activated
              .Include(p => p.AspNetUserCmpSignedBy)
              .Include(p => p.AspNetUserActivatedBy)
              .Include(p => p.Contact) //Customer
              .Include(p => p.Quote)
              .Include(p => p.Opportunity)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Priority)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Order Find(string id, string userId, string orgId)
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

        public async Task<Order> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Order Create(Order param, string userId, string orgId)
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

        private void Validate(Order param, AspNetUser applicationUser)
        {

        }

        public void Update(Order param, string userId, string orgId)
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

        public new void Delete(Order model)
        {
            //// Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                IRepository<OrderLineItem> lineItemRepository = LineItemRepository;

                //Delete details records
                var lineProducts = lineItemRepository.Query(p => p.OrderId == model.Id).Select();
                lineProducts.ForEach(lineItemRepository.Delete);

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

                throw exception;
            }
        }

        public IEnumerable<Order> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Name) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Name)
                        .ThenBy(c => c.Code))
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
