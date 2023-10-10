using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using Microsoft.Practices.ObjectBuilder2;

namespace Lifetrons.Erp.Service
{
    public class OpportunityService : Service<Opportunity>, IOpportunityService
    {
        [Dependency]
        public Repository<OpportunityLineItem> LineItemRepository { get; set; }

        private readonly IRepositoryAsync<Opportunity> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IUnitOfWork _unitOfWork;

        public OpportunityService(IRepositoryAsync<Opportunity> repository, IAspNetUserService aspNetUserService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByAccountAsync(DateTime startDateTime, DateTime endDateTime, string accountId, string orgId)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid accountIdGuid = accountId.ToSysGuid();

            var opportunities = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.AccountId == accountIdGuid & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.Campaign)
              .Include(p => p.LeadSource)
              .Include(p => p.Lead)
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Stage)
              .SelectAsync();

            return opportunities;
        }

        public async Task<IEnumerable<Opportunity>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var opportunities = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OwnerId == userId)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.Campaign)
              .Include(p => p.LeadSource)
              .Include(p => p.Lead)
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.DeliveryStatu)
              .Include(p => p.Stage)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return opportunities;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Opportunity Find(string id, string userId, string orgId)
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

        public async Task<Opportunity> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId  == applicationUser.OrgId) return record; 

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Opportunity Create(Opportunity param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Opportunity param, string userId, string orgId)
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

        public new void Delete(Opportunity model)
        {
            //// Begin transaction
            _unitOfWork.BeginTransaction();
            try
            {
                IRepository<OpportunityLineItem> lineItemRepository = LineItemRepository;

                //Delete details records
                var lineItems = lineItemRepository.Query(p => p.OpportunityId == model.Id).Select();
                lineItems.ForEach(lineItemRepository.Delete);

                //Delete master records 
                _repository.Delete(model);

                _unitOfWork.SaveChanges();

                //// Commit Transaction
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                //// Rollback transaction
                _unitOfWork.Rollback();

                throw;
            }
        }

        public IEnumerable<Opportunity> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
