using System;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class OpportunityLineItemService : Service<OpportunityLineItem>, IOpportunityLineItemService
    {
        private readonly IRepositoryAsync<OpportunityLineItem> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public OpportunityLineItemService(IRepositoryAsync<OpportunityLineItem> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            :base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        //public async Task<IEnumerable<OpportunityLineItem>> GetAsync(string userId, string orgId)
        //{
        //    Guid orgIdGuid = orgId.ToSysGuid();
        //    if (orgIdGuid == Guid.Empty)
        //    {
        //        throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
        //    }
        //    var task = await _repository.Query()
        //       .Include(p => p.AspNetUser)
        //       .Include(p => p.AspNetUser1)
        //       .Include(p => p.PriceBook)
        //       .Include(p => p.Product)
        //       .Filter(p => p.OrgId == orgIdGuid)
        //       .Filter(p => p.Active)
        //       .GetAsync();

        //    //Check user & org
        //    var applicationUser = await _aspNetUserService.FindAsync(userId);
        //    if (orgIdGuid == applicationUser.OrgId)
        //    {
        //        return task;
        //    }
        //    throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        //}

        public IEnumerable<OpportunityLineItem> SelectLineItems(string opportunityId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(opportunityId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = opportunityId.ToSysGuid();
            var task = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OpportunityId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Opportunity)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .OrderBy(q => q.OrderBy(c => c.Serial).ThenBy(c => c.Product.Name))
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return task;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<OpportunityLineItem>> SelectAsyncLineItems(string opportunityId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(opportunityId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = opportunityId.ToSysGuid();
            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.OpportunityId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Opportunity)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .OrderBy(q => q.OrderBy(c => c.Serial).ThenBy(c => c.Product.Name))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return task; 
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<OpportunityLineItem> FindAsync(string opportunityId, string priceBookId, string productId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();
            var record = await _repository.FindAsync(new object[] { opportunityId.ToSysGuid(), priceBookId.ToSysGuid(), productId.ToSysGuid() });
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId)
            {
                record.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        
        public OpportunityLineItem Create(OpportunityLineItem param, string userId, string orgId)
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

        public void Update(OpportunityLineItem param, string userId, string orgId)
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

        public void Delete(string id)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public IEnumerable<OpportunityLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Product.Code))
                    .OrderBy(q => q
                        .OrderBy(c => c.OpportunityId)
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
