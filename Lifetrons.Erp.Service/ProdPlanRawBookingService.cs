using System;
using System.Collections.Specialized;
using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Service
{
    public class ProdPlanRawBookingService : Service<ProdPlanRawBooking>, IProdPlanRawBookingService
    {
        [Dependency]
        public IProdPlanDetailService ProdPlanDetailService { get; set; }

        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IBOMLineItemService BOMLineItemService { get; set; }



        private readonly IRepositoryAsync<ProdPlanRawBooking> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;

        public ProdPlanRawBookingService(IRepositoryAsync<ProdPlanRawBooking> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProdPlanRawBooking> SelectLineItems(string prodPlanDetailId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(prodPlanDetailId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = prodPlanDetailId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProdPlanDetailId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<ProdPlanRawBooking>> SelectAsyncLineItems(string prodPlanDetailId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(prodPlanDetailId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = prodPlanDetailId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProdPlanDetailId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<ProdPlanRawBooking> SelectLineItems(DateTime startDateTime, DateTime endDateTime, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProdPlanDetail.StartDateTime >= startDateTime & p.ProdPlanDetail.StartDateTime <= endDateTime)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Item)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q
                        .OrderBy(c => c.ProdPlanDetail.StartDateTime)
                        .ThenBy(c => c.CreatedDate)
                        .ThenBy(c => c.JobNo))
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<ProdPlanRawBooking> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = await _repository.FindAsync(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (record == null) return null;
            if (record.OrgId == applicationUser.OrgId)
            {
                record.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProdPlanRawBooking Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var record = _repository.Find(new object[] { id.ToSysGuid() });

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (record == null) return null;

            if (record.OrgId == applicationUser.OrgId)
            {
                record.Organization = _organizationService.Find(orgId, userId, orgId);
                return record;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public bool Create(string prodPlanDetailId, string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(prodPlanDetailId) || string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid prodPlanDetailIdGuid = prodPlanDetailId.ToSysGuid();
            decimal jobNoDecimal = jobNo.ToJobNumber();
            var applicationUser = _aspNetUserService.Find(userId);

            // Fetch the booking records to check if booking already exists
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProdPlanDetailId == prodPlanDetailIdGuid)
              .Select();

            //Validations
            if (enumerable.Any()) throw new ApplicationException("Raw material booking already done for this planning detail record.");

            //Check if ProdPlandetail record is authorized
            var prodPlanDetail = ProdPlanDetailService.Find(prodPlanDetailIdGuid.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (prodPlanDetail.JobNo != jobNoDecimal) throw new ApplicationException("Planning Id and Job Number mismatch.");
            if (!prodPlanDetail.Authorized) throw new ApplicationException("Record not authorized. Please authorize the record to enable raw material booking and issue for production.");

            //// Begin transaction
            _unitOfWork.BeginTransaction();
            try
            {
                //Get The ProductId from ProdPlanDetail
                //Get Items(OperationBOMLineItem) for the Process(ProdPlan) and store against JobNo and ProdPlanDetailId(ProdPlanDetail)

                //var prodPlan = ProdPlanService.Find(prodPlanDetail.ProdPlanId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                var enterpriseStageId = (ProcessService.Find(prodPlanDetail.ProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).EnterpriseStageId;
                var bomLineItems = BOMLineItemService.SelectLineItems(prodPlanDetail.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                foreach (var item in bomLineItems)
                {
                    var param = new ProdPlanRawBooking
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = applicationUser.Id,
                        ProdPlanDetailId = prodPlanDetailIdGuid,
                        JobNo = Convert.ToDecimal(jobNo),
                        ItemId = item.ItemId,
                        Quantity = item.Quantity * prodPlanDetail.Quantity,
                        Weight = item.Weight * prodPlanDetail.Quantity,
                        WeightUnitId = item.WeightUnitId,
                        Remark = "Item expected on " + prodPlanDetail.StartDateTime.AddDays(-1).Date.ToLongDateString(),
                        OrgId = applicationUser.OrgId.ToSysGuid(),
                        CreatedBy = applicationUser.Id,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = applicationUser.Id,
                        ModifiedDate = DateTime.UtcNow,
                        Authorized = true,
                        Active = true
                    };

                    //Create
                    _repository.Insert(param);
                }

                //Update ProdPlanDetail IsRawMaterialBookedFlag
                prodPlanDetail.ObjectState = ObjectState.Modified;
                prodPlanDetail.ModifiedBy = userId;
                prodPlanDetail.ModifiedDate = DateTime.UtcNow;
                prodPlanDetail.IsRawBookingDone = true;
                prodPlanDetail.Remark += "\n" + applicationUser.UserName + ": RM Booked. ";
                ProdPlanDetailService.Update(prodPlanDetail, applicationUser.Id, applicationUser.OrgId.ToString());

                _unitOfWork.SaveChanges();

                //// Commit Transaction
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                //// Rollback transaction
                _unitOfWork.Rollback();

                throw new ApplicationException("Raw material booking failed.", exception); ;
            }

            return true;
        }

        public void Delete(string prodPlanDetailId)
        {
            //Do the validation here
            Guid prodPlanDetailIdGuid = prodPlanDetailId.ToSysGuid();

            //Do business logic here

            //// Begin transaction
            _unitOfWork.BeginTransaction();
            try
            {
                IRepository<ProdPlanRawBooking> lineItemRepository = _repository;

                //Delete details records
                var lineItems = lineItemRepository.Query(p => p.ProdPlanDetailId == prodPlanDetailIdGuid).Select();
                lineItems.ForEach(lineItemRepository.Delete);

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

        public IEnumerable<ProdPlanRawBooking> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Quantity.ToString()))
                    .OrderBy(q => q
                        .OrderBy(c => c.JobNo)
                        .ThenBy(c => c.Quantity))
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
