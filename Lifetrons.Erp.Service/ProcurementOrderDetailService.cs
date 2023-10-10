using System;
using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class ProcurementOrderDetailService : Service<ProcurementOrderDetail>, IProcurementOrderDetailService
    {
        [Dependency]
        public IProcurementRequestDetailService ProcurementRequestDetailService { get; set; }
        [Dependency]
        public IItemService ItemService { get; set; }

        private readonly IRepositoryAsync<ProcurementOrderDetail> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;


        public ProcurementOrderDetailService(IRepositoryAsync<ProcurementOrderDetail> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProcurementOrderDetail> SelectLineItems(string procurementOrderId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(procurementOrderId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = procurementOrderId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProcurementOrderId == listIdGuid)
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

        public async Task<IEnumerable<ProcurementOrderDetail>> SelectAsyncLineItems(string procurementOrderId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(procurementOrderId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = procurementOrderId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProcurementOrderId == listIdGuid)
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

        public async Task<ProcurementOrderDetail> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record == null) return null;

            if (record.OrgId == applicationUser.OrgId)
            {
                record.Item = await ItemService.FindAsync(record.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProcurementOrderDetail Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = _repository.Find(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (record == null) return null;

            if (record.OrgId == applicationUser.OrgId)
            {
                record.Item = ItemService.Find(record.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public decimal FindLastProcurementPriceOfItem(string itemId, string orgId)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(orgId)) throw new ApplicationException("Invalid parameters", new Exception("Invalid parameters.")); ;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid itemIdGuid = itemId.ToSysGuid();

            var records =  _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ItemId == itemIdGuid & p.Status == "Completed")
                            .OrderBy(q => q
                                    .OrderByDescending(c => c.ModifiedDate))
                           .Select();

            if (records == null) return 0;
            if(records.Count() > 0)
            {
                var record = records.FirstOrDefault();
                var price = (decimal) record.LineItemAmount / record.Quantity;
                return price;
            }
            else
            {
                return 0;
            }
        }
        public ProcurementOrderDetail Create(ProcurementOrderDetail param, string userId, string orgId)
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

        private void Validate(ProcurementOrderDetail param, AspNetUser applicationUser)
        {
            var procurementRequest = ProcurementRequestDetailService.Find(param.ProcurementRequestDetailId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            if (param.ItemId != procurementRequest.ItemId)
            {
                var item = ItemService.Find(procurementRequest.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                throw new ApplicationException("Item is different than the requested item. Item should be same as in procurement request. Requested Item is " + item.Code);
            }
        }

        public void Update(ProcurementOrderDetail param, string userId, string orgId)
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

        public IEnumerable<ProcurementOrderDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
                        .ThenBy(c => c.CreatedDate))
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
