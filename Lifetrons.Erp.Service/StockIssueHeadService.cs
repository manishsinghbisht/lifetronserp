using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Service
{
    public class StockIssueHeadService : Service<StockIssueHead>, IStockIssueHeadService
    {
        [Dependency]
        public Repository<StockItemIssue> StockItemLineItemRepository { get; set; }

        [Dependency]
        public Repository<StockProductIssue> StockProductLineItemRepository { get; set; }

        private readonly IRepositoryAsync<StockIssueHead> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IUnitOfWork _unitOfWork;

        public StockIssueHeadService(IRepositoryAsync<StockIssueHead> repository, IAspNetUserService aspNetUserService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<StockIssueHead> Select(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid quoteIdGuid = id.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == quoteIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.EnterpriseStage)
              .Include(p => p.Process)
              .Include(p => p.EnterpriseStage1)
              .Include(p => p.Process1)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<StockIssueHead>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Employee)
              .Include(p => p.EnterpriseStage)
              .Include(p => p.Process)
              .Include(p => p.EnterpriseStage1)
              .Include(p => p.Process1)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public StockIssueHead Find(string id, string userId, string orgId)
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

        public async Task<StockIssueHead> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public StockIssueHead Create(StockIssueHead param, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business validation
                Validate(param, applicationUser);

                //Create
                _repository.Insert(param);

                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        private void Validate(StockIssueHead param, AspNetUser applicationUser)
        {
            if (param.IssuedByProcessId == Guid.Empty || param.IssuedToProcessId== Guid.Empty)
                throw new ApplicationException("Empty process ids");

            if (param.IssuedByProcessId == param.IssuedToProcessId)
                throw new ApplicationException("Issued by and issued to processes cannot be same.");
        }

        public void Update(StockIssueHead param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business validation
                Validate(param, applicationUser);

                //Update
                _repository.Update(param);
            }
            else
            {
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            }
        }

        public new void Delete(StockIssueHead model)
        {
            //// Begin transaction
            _unitOfWork.BeginTransaction();
            try
            {
                IRepository<StockItemIssue> stockItemLineItemRepository = StockItemLineItemRepository;
                IRepository<StockProductIssue> stockProductLineItemRepository = StockProductLineItemRepository;

                //Delete details records
                var lineItems = stockItemLineItemRepository.Query(p => p.StockIssueId == model.Id).Select();
                lineItems.ForEach(stockItemLineItemRepository.Delete);

                var lineProducts = stockItemLineItemRepository.Query(p => p.StockIssueId == model.Id).Select();
                lineProducts.ForEach(stockProductLineItemRepository.Delete);

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



        public IEnumerable<StockIssueHead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Name) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Employee)
                        .ThenBy(c => c.EnterpriseStage)
                        .ThenBy(c => c.Process))
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
