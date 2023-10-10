using Lifetrons.Erp.Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class QuoteService : Service<Quote>, IQuoteService
    {
        private readonly IRepositoryAsync<Quote> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public QuoteService(IRepositoryAsync<Quote> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<Quote>> GetQuotesByAccountAsync(DateTime startDateTime, DateTime endDateTime, string accountId, string orgId)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid accountIdGuid = accountId.ToSysGuid();

            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.AccountId == accountIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.Contact)
              .Include(p => p.QuoteStatu)
              .Include(p => p.Opportunity)
              .SelectAsync();

            return task;
        }

        public IEnumerable<Quote> Select(string quoteId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid quoteIdGuid = quoteId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == quoteIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.Contact)
              .Include(p => p.QuoteStatu)
              .Include(p => p.Opportunity)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Quote>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OwnerId == userId)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.Account)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.Contact)
              .Include(p => p.QuoteStatu)
              .Include(p => p.Opportunity)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Quote Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = _repository.Find(id.ToSysGuid());
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (record == null) return null;
            if (record.OrgId == applicationUser.OrgId) return record;
            
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Quote> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(id.ToSysGuid());
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record == null) return null;
            if (record.OrgId == applicationUser.OrgId) return record;
            
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Quote Create(Quote param, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Quote param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId  == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
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

        public IEnumerable<Quote> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
