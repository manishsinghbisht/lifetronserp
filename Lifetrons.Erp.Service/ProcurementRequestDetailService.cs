using System;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class ProcurementRequestDetailService : Service<ProcurementRequestDetail>, IProcurementRequestDetailService
    {
        private readonly IRepositoryAsync<ProcurementRequestDetail> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;

        public ProcurementRequestDetailService(IRepositoryAsync<ProcurementRequestDetail> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProcurementRequestDetail> SelectLineItems(string procurementRequestId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(procurementRequestId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = procurementRequestId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProcurementRequestId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<ProcurementRequestDetail>> SelectAsyncLineItems(string procurementRequestId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(procurementRequestId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = procurementRequestId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProcurementRequestId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<ProcurementRequestDetail>> SelectAsyncPendingLineItems(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Status.ToUpper() == "PENDING")
                .OrderBy(q => q
                        .OrderBy(c => c.RequiredByDate)
                        .ThenBy(c => c.ItemId))
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

       public async Task<ProcurementRequestDetail> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record == null) return null;
            if (record.OrgId == applicationUser.OrgId)
            {
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProcurementRequestDetail Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = _repository.Find(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (record == null) return null;

            if (record.OrgId == applicationUser.OrgId)
            {
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProcurementRequestDetail Create(ProcurementRequestDetail param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

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

        private void Validate(ProcurementRequestDetail param, AspNetUser applicationUser)
        {

        }

        public void Update(ProcurementRequestDetail param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

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

        public void Delete(string id)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public IEnumerable<ProcurementRequestDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Quantity.ToString()))
                    .OrderBy(q => q
                        .OrderBy(c => c.Serial)
                        .ThenBy(c => c.ItemId)
                        .ThenBy(c => c.RequiredByDate))
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
