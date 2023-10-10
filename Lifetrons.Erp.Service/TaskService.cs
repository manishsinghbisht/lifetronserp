using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class TaskService : Service<Lifetrons.Erp.Data.Task>, ITaskService
    {
        private readonly IRepositoryAsync<Lifetrons.Erp.Data.Task> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        [Dependency]
        public IAccountService AccountService { get; set; }

        [Dependency]
        public IContactService ContactService { get; set; }

        [Dependency]
        public ICampaignService CampaignService { get; set; }

        [Dependency]
        public IOpportunityService OpportunityService { get; set; }

        [Dependency]
        public IQuoteService QuoteService { get; set; }

        [Dependency]
        public IOrderService OrderService { get; set; }

        [Dependency]
        public ICaseService CaseService { get; set; }

        [Dependency]
        public IDispatchService DispatchService { get; set; }

        public TaskService(IRepositoryAsync<Lifetrons.Erp.Data.Task> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Lifetrons.Erp.Data.Task>> SelectAsyncTasksByAccount(DateTime startDateTime, DateTime endDateTime, string accountId, string orgId)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid accountIdGuid = accountId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.RelatedToId == accountIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
             .Include(p => p.AspNetUser)//Created
              .Include(p => p.AspNetUser1)//Modified
              .Include(p => p.AspNetUser2)//OwnerId
              .Include(p => p.AspNetUserReportCompletionTo)//ReportCompletionToId
              .Include(p => p.Lead)
              .Include(p => p.Contact)
              .Include(p => p.TaskStatu)
              .Include(p => p.Priority)
              .SelectAsync();

            return enumerable;
        }

        public async Task<IEnumerable<Lifetrons.Erp.Data.Task>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.OwnerId == userId | p.ReportCompletionToId == userId))
              .Include(p => p.AspNetUser)//Created
              .Include(p => p.AspNetUser1)//Modified
              .Include(p => p.AspNetUser2)//OwnerId
              .Include(p => p.AspNetUserReportCompletionTo)//ReportCompletionToId
              .Include(p => p.Lead)
              .Include(p => p.Contact)
              .Include(p => p.TaskStatu)
              .Include(p => p.Priority)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Lifetrons.Erp.Data.Task> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Lifetrons.Erp.Data.Task Create(Lifetrons.Erp.Data.Task param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId  == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Lifetrons.Erp.Data.Task param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
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

        public IEnumerable<Lifetrons.Erp.Data.Task> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
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

        public IEnumerable<Lifetrons.Erp.Data.Task> Select(DateTime startDateTime, DateTime endDateTime, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var task = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OwnerId == userId & (p.StartDate >= startDateTime && p.EndDate <= endDateTime))
              .Include(p => p.AspNetUser)//Created
              .Include(p => p.AspNetUser1)//Modified
              .Include(p => p.AspNetUser2)//OwnerId
              .Include(p => p.AspNetUserReportCompletionTo)//ReportCompletionToId
              .Include(p => p.Lead)
              .Include(p => p.Contact)
              .Include(p => p.TaskStatu)
              .Include(p => p.Priority)
              .Include(p => p.Organization)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public string GetRelatedToIdName(string relatedToObjectName, string relatedToId, string userId, string orgId)
        {
            string relatedToIdName = string.Empty;
            try
            {
                switch (relatedToObjectName)
                {
                    case "Account":
                        relatedToIdName = AccountService.Find(relatedToId, userId, orgId).Name;
                        break;
                    case "Contact":
                        relatedToIdName = ContactService.Find(relatedToId, userId, orgId).Name;
                        break;
                    case "Opportunity":
                        relatedToIdName = OpportunityService.Find(relatedToId, userId, orgId).Name;
                        break;
                    case "Order":
                        relatedToIdName = OrderService.Find(relatedToId, userId, orgId).Name;
                        break;
                    case "Quote":
                        relatedToIdName = QuoteService.Find(relatedToId, userId, orgId).Name;
                        break;
                    case "Campaign":
                        relatedToIdName = CampaignService.Find(relatedToId, userId, orgId).Name;
                        break;
                    case "Case":
                        relatedToIdName = CaseService.Find(relatedToId, userId, orgId).Name;
                        break;
                    case "Dispatch":
                        relatedToIdName = DispatchService.Find(relatedToId, userId, orgId).Name;
                        break;
                }
            }
            catch (Exception exception)
            {
                relatedToIdName = string.Empty;
            }

            return relatedToIdName;
        }

        public void Dispose()
        {
        }

    }
}
