using Lifetrons.Erp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class OperationBOMLineItemService : Service<OperationBOMLineItem>, IOperationBOMLineItemService
    {
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IRepositoryAsync<OperationBOMLineItem> _repository;
        private readonly IOrganizationService _organizationService;

        public OperationBOMLineItemService(IRepositoryAsync<OperationBOMLineItem> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<OperationBOMLineItem> SelectLineItems(string productId, string enterpriseStageId, string processId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid productIdGuid = productId.ToSysGuid();
            Guid enterpriseStageIdGuid = enterpriseStageId.ToSysGuid();
            Guid processIdGuid = processId.ToSysGuid();

            var operationBomLineItems = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProductId == productIdGuid & p.EnterpriseStageId == enterpriseStageIdGuid & p.ProcessId == processIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .Include(p => p.WeightUnit1)
               .Include(p => p.BOMLineItem)
               .Include(p => p.BOMLineItem.Item)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return operationBomLineItems;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public OperationBOMLineItem Find(string productId, string enterpriseStageId, string processId, string itemId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();
            var operationBomLineItem = _repository.Find(new object[] { productId.ToSysGuid(), enterpriseStageId.ToSysGuid(), processId.ToSysGuid(), itemId.ToSysGuid() });
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (operationBomLineItem == null) return null;
            if (operationBomLineItem.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                operationBomLineItem.Organization = _organizationService.Find(orgId, userId, orgId);
                return operationBomLineItem;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<OperationBOMLineItem> FindAsync(string productId, string enterpriseStageId, string processId, string itemId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();
            var operationBomLineItem = await _repository.FindAsync(new object[] { productId.ToSysGuid(), enterpriseStageId.ToSysGuid(), processId.ToSysGuid(), itemId.ToSysGuid() });
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (operationBomLineItem == null) return operationBomLineItem;
            if (operationBomLineItem.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                operationBomLineItem.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return operationBomLineItem;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public OperationBOMLineItem Create(OperationBOMLineItem param, string userId, string orgId)
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

        public void Update(OperationBOMLineItem param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
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

        public void Delete(OperationBOMLineItem param)
        {
            _repository.Delete(param);
        }

        public IEnumerable<OperationBOMLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
