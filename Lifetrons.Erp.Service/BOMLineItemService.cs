using Lifetrons.Erp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class BOMLineItemService : Service<BOMLineItem>, IBOMLineItemService
    {
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IRepositoryAsync<BOMLineItem> _repository;
        private readonly IOrganizationService _organizationService;

        public BOMLineItemService(IRepositoryAsync<BOMLineItem> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<BOMLineItem> SelectLineItems(string productId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = productId.ToSysGuid();
            var bomLineItems = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProductId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return bomLineItems;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public BOMLineItem Find(string productId, string itemId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();
            var bomLineItem = _repository.Find(new object[] { productId.ToSysGuid(), itemId.ToSysGuid() });
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (bomLineItem == null) return bomLineItem;
            if (bomLineItem.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                bomLineItem.Organization = _organizationService.Find(orgId, userId, orgId);
                return bomLineItem;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<BOMLineItem> FindAsync(string productId, string itemId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();
            var bomLineItem = await _repository.FindAsync(new object[] { productId.ToSysGuid(), itemId.ToSysGuid() });
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (bomLineItem == null) return bomLineItem;
            if (bomLineItem.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                bomLineItem.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return bomLineItem;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public BOMLineItem Create(BOMLineItem param, string userId, string orgId)
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

        public void Update(BOMLineItem param, string userId, string orgId)
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

        public void Delete(BOMLineItem param)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(param);
        }

        public IEnumerable<BOMLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Quantity)
                        .ThenBy(c => c.Weight))
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
