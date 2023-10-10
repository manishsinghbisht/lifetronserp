using Lifetrons.Erp.Data;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Service
{
    public class OperationService : Service<Operation>, IOperationService
    {
        [Dependency]
        public Repository<OperationBOMLineItem> OperationBOMLineItemRepository { get; set; }

        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }

        private readonly IRepositoryAsync<Operation> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public OperationService(IRepositoryAsync<Operation> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<Operation>> SelectLineItemsAsync(string productId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = productId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProductId == listIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.EnterpriseStage)
              .Include(p => p.Process)
              .Include(p => p.Product)
              .OrderBy(p => p
                        .OrderBy(c => c.Serial)
                        .ThenBy(c => c.ProcessId)
                        .ThenBy(c => c.Process.Serial))
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Operation> FindAsync(string productId, string enterpriseStageId, string processId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(enterpriseStageId) || string.IsNullOrEmpty(processId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var task = await _repository.FindAsync(new object[] { productId.ToSysGuid(), enterpriseStageId.ToSysGuid(), processId.ToSysGuid() });

            return task;
        }

        public Operation Create(Operation param, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                if (param.CycleTimeInHour <= 0)
                {
                    param.CycleTimeInHour = (Decimal)0.0166667; //1 minute = 0.0166667 hour
                }
                if (param.CycleCapacity <= 0)
                {
                    param.CycleCapacity = 1;
                }
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Operation param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business cases
                if (param.CycleTimeInHour <= 0)
                {
                    param.CycleTimeInHour = (Decimal)0.0166667; //1 minute = 0.0166667 hour
                }
                if (param.CycleCapacity <= 0)
                {
                    param.CycleCapacity = 1;
                }
                //Update
                _repository.Update(param);
            }
            else
            {
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            }
        }

        public void Delete(Operation param)
        {
            //// Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                IRepository<OperationBOMLineItem> lineItemRepository = OperationBOMLineItemRepository;

                //Delete details records
                var lineItems = lineItemRepository.Query(p => (p.OperationId == param.Id)).Select();
                lineItems.ForEach(lineItemRepository.Delete);

                //Delete master records 
                _repository.Delete(param);

                UnitOfWork.SaveChanges();

                //// Commit Transaction
                UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                //// Rollback transaction
                UnitOfWork.Rollback();

                throw;
            }
        }

        public void Dispose()
        {
        }

    }
}
