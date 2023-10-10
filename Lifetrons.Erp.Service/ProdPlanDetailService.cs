using System;
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

namespace Lifetrons.Erp.Service
{
    public class ProdPlanDetailService : Service<ProdPlanDetail>, IProdPlanDetailService
    {
        
        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }

        [Dependency]
        public IJobReceiptHeadService JobReceiptHeadService { get; set; }

        [Dependency]
        public IJobProductReceiptService JobProductReceiptService { get; set; }

        private readonly IRepositoryAsync<ProdPlanDetail> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;

        public ProdPlanDetailService(IRepositoryAsync<ProdPlanDetail> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProdPlanDetail>> SelectAsyncLineItems(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            decimal jobNoDecimal = Convert.ToDecimal(jobNo);
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q
                        .OrderBy(c => c.StartDateTime)
                        .ThenBy(c => c.Serial)
                        .ThenBy(c => c.JobNo))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<ProdPlanDetail>> SelectAsyncLineItems(DateTime startDateTime, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StartDateTime >= startDateTime)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q
                        .OrderBy(c => c.StartDateTime)
                        .ThenBy(c => c.Serial)
                        .ThenBy(c => c.JobNo))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<ProdPlanDetail>> SelectAsyncLineItems(DateTime startDateTime, DateTime endDateTime, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.StartDateTime >= startDateTime & p.EndDateTime <= endDateTime)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q
                        .OrderBy(c => c.StartDateTime)
                        .ThenBy(c => c.Serial)
                        .ThenBy(c => c.JobNo))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return enumerable;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<ProdPlanDetail>> SelectAsyncActionableItems(string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.IsIssuedForProduction == false | p.IsRawBookingDone == false))
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q
                        .OrderBy(c => c.StartDateTime)
                        .ThenBy(c => c.Serial)
                        .ThenBy(c => c.JobNo))
               .SelectAsync();

            return enumerable;
        }
        public decimal QuantityInPlanning(string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters missing.", new Exception("Parameters missing."));

            Guid orgIdGuid = orgId.ToSysGuid();
            decimal jobNoDecimal = Convert.ToDecimal(jobNo);

            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.JobNo == jobNoDecimal)
               .Select();

            decimal sum = enumerable.Sum(x => x.Quantity);

            return sum;
        }

        public decimal QuantityInProduction(string jobNo, string userId, string orgId)
        {
            decimal quantityInProduction = JobProductReceiptService.TotalQuantityReceipt(jobNo,
                       Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"],
                       Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Planning"],
                       userId,
                       orgId);
            return quantityInProduction;
        }

        public bool IssueForProduction(string prodPlanDetailId, string jobNo, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(prodPlanDetailId) || string.IsNullOrEmpty(jobNo) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid prodPlanDetailIdGuid = prodPlanDetailId.ToSysGuid();
            decimal jobNoDecimal = jobNo.ToJobNumber();
            var applicationUser = _aspNetUserService.Find(userId);

            //Check if ProdPlandetail record is authorized
            var prodPlanDetail = Find(prodPlanDetailIdGuid.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (prodPlanDetail.JobNo != jobNoDecimal) throw new ApplicationException("Planning Id and Job Number mismatch.");
            if (!prodPlanDetail.Authorized) throw new ApplicationException("Record not authorized. Please authorize the record to enable raw material booking and issue for production.");
            // Fetch the quantityInProduction
            decimal quantityInPlanning = QuantityInPlanning(jobNo, applicationUser.Id, applicationUser.OrgId.ToString());
            decimal quantityInProduction = QuantityInProduction(jobNo, applicationUser.Id, applicationUser.OrgId.ToString());
            // Fetch order production quantity
            var orderLineItem = OrderLineItemService.SelectSingle(jobNo, applicationUser.Id, applicationUser.OrgId.ToString());
            decimal orderProductionQuantity = orderLineItem.ProductionQuantity ?? 0;
            //Calculate quanity available for issueing to production
            decimal quantityAvailable = quantityInPlanning - quantityInProduction;
            if (prodPlanDetail.Quantity > quantityAvailable)
                throw new ApplicationException("Quantity exceeds the balance quantity available for issue to production. \r\n "
                     + " Production quantity in work order: " + orderProductionQuantity + ". \r\n "
                    + " Quantity already issued to Production Assembly process: " + quantityInProduction + ". \r\n "
                    + " Available quantity: " + quantityAvailable);

            //// Begin transaction
            _unitOfWork.BeginTransaction();
            try
            {
                //Create master record
                var paramMaster = new JobReceiptHead()
                {
                    Id = Guid.NewGuid(),
                    Name = Helper.SysSeparator + DateTime.UtcNow,
                    OrgId = applicationUser.OrgId.ToSysGuid(),
                    CreatedBy = applicationUser.Id,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = applicationUser.Id,
                    ModifiedDate = DateTime.UtcNow,
                    Authorized = true,
                    Active = true,
                    Remark = "Issued for production: This is system created receipt in Assembly(Production) from Planning(Planning). Job No:" + jobNo,  
                    ReceiptByProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid(),
                    ReceiptFromProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Planning"].ToSysGuid(),
                    JobType = "I",
                    Date = DateTime.UtcNow,
                };

                JobReceiptHeadService.Create(paramMaster, userId, orgId);

                //Create Detail record
                var paramDetail = new JobProductReceipt()
                {
                    Id = Guid.NewGuid(),
                    JobReceiptId = paramMaster.Id,
                    Serial = 1,
                    JobNo = jobNoDecimal,
                    Quantity = prodPlanDetail.Quantity,
                    Weight = prodPlanDetail.Weight,
                    WeightUnitId = prodPlanDetail.WeightUnitId,
                    ExtraQuantity = 0,
                    LabourCharge = 0,
                    Expenses = 0,
                    OtherCharge = 0,
                    Remark = "Issued for production: This is system created product receipt in Assembly(Production) from Planning(Planning). Job No:" + jobNo,                    
                    OrgId = applicationUser.OrgId.ToSysGuid(),
                    CreatedBy = applicationUser.Id,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = applicationUser.Id,
                    ModifiedDate = DateTime.UtcNow,
                    Authorized = true,
                    Active = true,
                };

                JobProductReceiptService.Create(paramDetail, userId, orgId);

                //Update ProdPlanDetail IsRawMaterialBookedFlag
                prodPlanDetail.ObjectState = ObjectState.Modified;
                prodPlanDetail.ModifiedBy = userId;
                prodPlanDetail.ModifiedDate = DateTime.UtcNow;
                prodPlanDetail.IsIssuedForProduction = true;
                prodPlanDetail.Remark += "\n" + applicationUser.UserName + ": Issued for production. ";
                Update(prodPlanDetail, applicationUser.Id, applicationUser.OrgId.ToString());

                _unitOfWork.SaveChanges();

                //// Commit Transaction
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                //// Rollback transaction
                _unitOfWork.Rollback();

                throw new ApplicationException("Issue for production failed.", exception); ;
            }

            return true;
        }
        public async Task<ProdPlanDetail> FindAsync(string id, string userId, string orgId)
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

        public ProdPlanDetail Find(string id, string userId, string orgId)
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

        public ProdPlanDetail Create(ProdPlanDetail param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            //Business validation
            
            //Link ProcessId. Do it only on Create and NOT in Edit
            param.ProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();

            Validate(param, applicationUser);

            //Check user & org
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Link with OrderLineItem through JobNo/OrderLineItemId
                var orderLineItem = OrderLineItemService.SelectSingle(param.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                param.OrderLineItemId = orderLineItem.Id.ToSysGuid();
                param.OrderId = orderLineItem.OrderId;
                param.PriceBookId = orderLineItem.PriceBookId;
                param.ProductId = orderLineItem.ProductId;

                //Create
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        private void Validate(ProdPlanDetail param, AspNetUser applicationUser)
        {
            //Job dates should not exceed parent prodplan dates
            //Check start and end dates
            if (param.StartDateTime >= param.EndDateTime) throw new ApplicationException("Start date time should be less than End date time");

            //Check negative quanity
            //Check start and end dates
            if (param.Quantity < 1) throw new ApplicationException("Quantity cannot be less than 1.");

            //Get the balance quantity available for planning
            decimal quantityInPlanning = QuantityInPlanning(param.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            decimal quantityInProduction = JobProductReceiptService.TotalQuantityReceipt(param.JobNo.ToString(),
                        Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"],
                        Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Planning"],
                        applicationUser.Id,
                        applicationUser.OrgId.ToString());
            var orderLineItem = OrderLineItemService.SelectSingle(param.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            decimal productionQuantity = orderLineItem.ProductionQuantity ?? 0;
            decimal quantityUnPlanned = productionQuantity - quantityInPlanning;
            if (param.Quantity > quantityUnPlanned)
                throw new ApplicationException("Quantity exceeds the balance quantity available for job number in order. \r\n "
                     + " Production quantity in work order: " + productionQuantity + ". \r\n "
                     + " Quantity already in planning process: " + quantityInPlanning + ". \r\n "
                    + " Quantity already issued to Assembly process: " + quantityInProduction + ". \r\n "
                    + " Available quantity: " + quantityUnPlanned);
        }

        public void Update(ProdPlanDetail param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);

            //Business validation
            Validate(param, applicationUser);

            //Check user & org
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Link with OrderLineItem through JobNo/OrderLineItemId
                var orderLineItem = OrderLineItemService.SelectSingle(param.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                param.OrderLineItemId = orderLineItem.Id.ToSysGuid();
                param.OrderId = orderLineItem.OrderId;
                param.PriceBookId = orderLineItem.PriceBookId;
                param.ProductId = orderLineItem.ProductId;

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
            var param = Find(id.ToSysGuid());
            if(param.IsIssuedForProduction)
                throw new ApplicationException("Cannot delete. Already issued in production.");
            if (param.IsRawBookingDone)
                throw new ApplicationException("Cannot delete. Raw material already booked.");
            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public IEnumerable<ProdPlanDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Quantity.ToString()))
                    .OrderBy(q => q
                        .OrderBy(c => c.Serial)
                        .ThenBy(c => c.JobNo)
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
