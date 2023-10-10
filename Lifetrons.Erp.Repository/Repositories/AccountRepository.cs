using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Models;
using Repository.Pattern.Repositories;

namespace Lifetrons.Erp.Repository.Repositories
{
    // Exmaple: How to add custom methods to a repository.
    public static class AccountRepository
    {
        public static decimal GetCustomerOrderTotalByYear(this IRepository<Account> repository, Guid Id, int year)
        {
            return repository
                .Queryable()
                .Where(c => c.Id == Id)
                .SelectMany(c => c.Orders.Where(o => o.DeliveryDate != null && o.DeliveryDate.Value.Year == year))
                .SelectMany(c => c.OrderLineItems)
                .Select(c => c.Quantity*c.SalesPrice)
                .Sum();
        }

        public static IEnumerable<Account> AccountByCompany(this IRepositoryAsync<Account> repository, string code)
        {
            return repository
                .Queryable()
                .Where(x => x.Code.Contains(code))
                .AsEnumerable();
        }

        public static IEnumerable<AccountOrder> GetAccountOrder(this IRepository<Account> repository, string country)
        {
            var accounts = repository.GetRepository<Account>().Queryable();
            var orders = repository.GetRepository<Order>().Queryable();

            var query = from c in accounts
                join o in orders on new {a = c.Id, b = c.ShippingAddressCountry}
                    equals new {a = o.Id, b = country}
                select new AccountOrder
                {
                    Id = c.Id,
                    Name = c.Name,
                    OrderId = o.Id,
                    OrderDate = o.DeliveryDate
                };

            return query.AsEnumerable();
        }

        public static IEnumerable<Account> GetAccounts(this IRepository<Account> repository, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            return repository.Queryable()
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.AspNetUser2)
               .Include(p => p.Address)
               .Include(p => p.AccountType)
               .Include(p => p.Industry)
               .Include(p => p.Ownership)
               .Where(p => p.Active & p.OrgId == orgIdGuid)
               .AsEnumerable();
        }
    }
}