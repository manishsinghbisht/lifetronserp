using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Queries;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Service
{
   public class AccountService : Service<Account>, IAccountService
    {
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IRepositoryAsync<Account> _repository;

        public AccountService(IRepositoryAsync<Account> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<Account>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
                    .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
                    .Include(p => p.AspNetUser)
                    .Include(p => p.AspNetUser1)
                    .Include(p => p.AspNetUser2)
                    .Include(p => p.Address)
                    .Include(p => p.AccountType)
                    .Include(p => p.Industry)
                    .Include(p => p.Ownership)
                    .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Account Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var account = _repository.Find(id.ToSysGuid());
            if (account == null) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (account.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return account;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Account> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var task = await _repository.FindAsync(id.ToSysGuid());
            if (task == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<Account> AccountsWithPurchasesMorethan(decimal amount)
        {
            var enumerable = _repository.Query(new AccountSalesQuery()
                .WithPurchasesMoreThan(amount)).Select();

            return enumerable;
        }
        public IEnumerable<Account> AccountsWithQuantitiesMorethan(decimal quantity)
        {
            var enumerable = _repository.Query(new AccountSalesQuery()
                .WithQuantitiesMoreThan(quantity)).Select();

            return enumerable;
        }

        public Account Create(Account param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Account param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Update
                _repository.Update(param);
            }
            else
            {
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            }
        }

        //public void Delete(string id)
        //{
        //    //Do the validation here

        //    //Do business logic here

        //    _repository.Delete(id.ToSysGuid());
        //}

        public IEnumerable<Account> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
