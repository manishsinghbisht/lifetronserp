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
    public class CaseService : Service<Case>, ICaseService
    {
        private readonly IRepositoryAsync<Case> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public CaseService(IRepositoryAsync<Case> repository, IAspNetUserService aspNetUserService) : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<Case>> SelectAsyncCasesByAccount(DateTime startDateTime, DateTime endDateTime, string accountId, string orgId)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid accountIdGuid = accountId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.AccountId == accountIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
               .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.CaseReason)
              .Include(p => p.CaseStatu)
              .Include(p => p.Priority)
              .Include(p => p.Account)
              .Include(p => p.Contact)
              .Include(p => p.Quote)
              .Include(p => p.Invoice)
              .Include(p => p.Product)
              .Include(p => p.Order)
              .Include(p => p.Opportunity)
              .Include(p => p.Campaign)
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<Case> Select(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.OwnerId == userId | p.ReportCompletionToId == userId))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.CaseReason)
              .Include(p => p.CaseStatu)
              .Include(p => p.Priority)
              .Include(p => p.Account)
              .Include(p => p.Contact)
              .Include(p => p.Quote)
              .Include(p => p.Invoice)
              .Include(p => p.Product)
              .Include(p => p.Order)
              .Include(p => p.Opportunity)
              .Include(p => p.Campaign)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Case>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.OwnerId == userId | p.ReportCompletionToId == userId))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.CaseReason) 
              .Include(p => p.CaseStatu)
              .Include(p => p.Priority)
              .Include(p => p.Account)
              .Include(p => p.Contact) 
              .Include(p => p.Quote)
              .Include(p => p.Invoice)
              .Include(p => p.Product)
              .Include(p => p.Order)
              .Include(p => p.Opportunity)
              .Include(p => p.Campaign)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Case>> SelectAsyncAllCases(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.CaseReason)
              .Include(p => p.CaseStatu)
              .Include(p => p.Priority)
              .Include(p => p.Account)
              .Include(p => p.Contact)
              .Include(p => p.Quote)
              .Include(p => p.Invoice)
              .Include(p => p.Product)
              .Include(p => p.Order)
              .Include(p => p.Opportunity)
              .Include(p => p.Campaign)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Case> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            
            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Case Find(string id, string userId, string orgId)
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

        public Case Create(Case param, string userId, string orgId)
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

        public void Update(Case param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");
         
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

        //public void Delete(string id)
        //{
        //    //Do the validation here

        //    //Do business logic here

        //    _repository.Delete(id.ToSysGuid());
        //}

        public IEnumerable<Case> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
