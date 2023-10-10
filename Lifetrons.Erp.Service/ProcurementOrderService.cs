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
    public class ProcurementOrderService : Service<ProcurementOrder>, IProcurementOrderService
    {
        [Dependency]
        public Repository<ProcurementOrderDetail> LineItemRepository { get; set; }

        private readonly IRepositoryAsync<ProcurementOrder> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ProcurementOrderService(IRepositoryAsync<ProcurementOrder> repository, IAspNetUserService aspNetUserService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProcurementOrder> Select(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid quoteIdGuid = id.ToSysGuid();

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == quoteIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Contact)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.Address2)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<ProcurementOrder>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.Account)
              .Include(p => p.Contact)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.Address2)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProcurementOrder Find(string id, string userId, string orgId)
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

        public async Task<ProcurementOrder> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(id.ToSysGuid());
            if (record == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record.OrgId == applicationUser.OrgId) return record;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProcurementOrder Create(ProcurementOrder param, string userId, string orgId)
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

        private void Validate(ProcurementOrder param, AspNetUser applicationUser)
        {

        }

        public void Update(ProcurementOrder param, string userId, string orgId)
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

        public new void Delete(ProcurementOrder model)
        {
            //// Begin transaction
            _unitOfWork.BeginTransaction();
            try
            {
                IRepository<ProcurementOrderDetail> lineItemRepository = LineItemRepository;

                //Delete details records
                var lineItems = lineItemRepository.Query(p => p.ProcurementOrderId == model.Id).Select();
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



        public IEnumerable<ProcurementOrder> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Name) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Date)
                        .ThenBy(c => c.Account.Name)
                        .ThenBy(c => c.DeliveryDate))
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
