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
using Microsoft.Practices.ObjectBuilder2;

namespace Lifetrons.Erp.Service
{
    public class DispatchService : Service<Dispatch>, IDispatchService
    {
        [Dependency]
        public Repository<DispatchLineItem> LineItemRepository { get; set; }

        private readonly IRepositoryAsync<Dispatch> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IUnitOfWork _unitOfWork;

        public DispatchService(IRepositoryAsync<Dispatch> repository, IAspNetUserService aspNetUserService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Dispatch> Select(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid quoteIdGuid = id.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == quoteIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Order)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Contact)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Dispatch>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Order)
              .Include(p => p.Account)
              .Include(p => p.Account1)
              .Include(p => p.Contact)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Dispatch Find(string id, string userId, string orgId)
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

        public async Task<Dispatch> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Dispatch Create(Dispatch param, string userId, string orgId)
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

        public void Update(Dispatch param, string userId, string orgId)
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

        public new void Delete(Dispatch model)
        {
            //// Begin transaction
            _unitOfWork.BeginTransaction();
            try
            {
                IRepository<DispatchLineItem> lineItemRepository = LineItemRepository;

                //Delete details records
                var lineItems = lineItemRepository.Query(p => p.DispatchId == model.Id).Select();
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



        public IEnumerable<Dispatch> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Name) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.AccountId)
                        .ThenBy(c => c.SubAccountId)
                        .ThenBy(c => c.OrderId))
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
