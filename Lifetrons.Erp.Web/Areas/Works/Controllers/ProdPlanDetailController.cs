using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Lifetrons.Erp.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class ProdPlanDetailController : BaseController
    {
        private readonly IProdPlanDetailService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IWeightUnitService _weightUnitService;

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IProdPlanRawBookingService ProdPlanRawBookingService { get; set; }

        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }

        // GET: /ProdPlanDetail/
        public ProdPlanDetailController(IProdPlanDetailService service, IUnitOfWorkAsync unitOfWork, IWeightUnitService weightUnitService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _weightUnitService = weightUnitService;
        }

        //public async Task<ActionResult> Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            int? page = 1;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            DateTime startDateTime = Convert.ToDateTime(DateTime.Now.AddDays(-15));
            DateTime endDateTime = Convert.ToDateTime(DateTime.Now.AddDays(15));

            IPagedList<MasterProductionScheduleViewModel> enumerablePage = await JobScheduling(page, applicationUser, startDateTime, endDateTime);

            return View(enumerablePage);
        }
               

        [HttpPost]
        public async Task<ActionResult> Index(string startDate, string endDate, int? page)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(startDate))
            {
                return View();
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            DateTime startDateTime = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDateTime = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());
            if (startDateTime > endDateTime)
            {
                endDateTime = startDateTime.AddDays(1);
            }

            IPagedList<MasterProductionScheduleViewModel> enumerablePage = await JobScheduling(page, applicationUser, startDateTime, endDateTime);
            return View(enumerablePage);
        }

        private async Task<IPagedList<MasterProductionScheduleViewModel>> JobScheduling(int? page, ApplicationUser applicationUser, DateTime startDateTime, DateTime endDateTime)
        {
            //Get the records as per selected criteria
            var records = await _service.SelectAsyncLineItems(startDateTime, endDateTime, applicationUser.Id, applicationUser.OrgId.ToString());
            //Get the records for which actions are pending
            var actionableRecords = await _service.SelectAsyncActionableItems(applicationUser.OrgId.ToString());

            var masterProductionScheduleViewModels = new List<MasterProductionScheduleViewModel>();

            //Add the date selection records
            if (records.Any())
            {
                records.ForEach(p =>
                {
                    CreateMPSViewModel(p, applicationUser, masterProductionScheduleViewModels);
                });
            }

            //Append the actionable records
            if (actionableRecords.Any())
            {
                actionableRecords.ForEach(p =>
                {
                    CreateMPSViewModel(p, applicationUser, masterProductionScheduleViewModels);
                });
            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = masterProductionScheduleViewModels.ToPagedList(pageNumber, 50); // will only contain 50 products max because of the pageSize
            return enumerablePage;
        }

        private void CreateMPSViewModel(ProdPlanDetail p, ApplicationUser applicationUser,
            List<MasterProductionScheduleViewModel> masterProductionScheduleViewModels)
        {

            //Get the balance quantity available for planning
            decimal quantityPlanned = _service.QuantityInPlanning(p.JobNo.ToString(), applicationUser.Id,
                applicationUser.OrgId.ToString());
            decimal quantityInProduction = _service.QuantityInProduction(p.JobNo.ToString(), applicationUser.Id,
                applicationUser.OrgId.ToString());
            var orderLineItem = OrderLineItemService.SelectSingle(p.JobNo.ToString(), applicationUser.Id,
                applicationUser.OrgId.ToString());
            decimal orderProductionQuantity = orderLineItem.ProductionQuantity ?? 0;
            decimal quantityUnPlanned = orderProductionQuantity - quantityPlanned;

            var viewModel = new MasterProductionScheduleViewModel
            {
                Id = p.Id,
                StartDateTime = p.StartDateTime,
                EndDateTime = p.EndDateTime,
                Serial = p.Serial,
                JobNo = p.JobNo,
                Quantity = p.Quantity,
                Weight = p.Weight,
                WeightUnitId = p.WeightUnitId,
                WeightUnit = p.WeightUnit,
                IsRawBookingDone = p.IsRawBookingDone,
                IsIssuedForProduction = p.IsIssuedForProduction,
                QuantityUnplanned = quantityUnPlanned,
                QuantityPlanned = quantityPlanned,
                QuantityInProduction = quantityInProduction,
                OrderQuantity = orderProductionQuantity
            };

            masterProductionScheduleViewModels.Add(viewModel);
        }

        // GET: /ProdPlanDetail/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = await AspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await AspNetUserService.FindAsync(instance.ModifiedBy); // Modified
            instance.WeightUnit = await _weightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(instance);
        }

        // GET: /ProdPlanDetail/Create
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /ProdPlanDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Create([Bind(Include = "Serial,JobNo,Quantity,Weight,WeightUnitId,StartDateTime,EndDateTime,CycleTimeInHour,CycleCapacity,JobType,SetupTimeInHrs,AddOnTimeInHrs,ActualStartDateTime,ActualEndDateTime,Remark,Authorized,CustomColumn1,ColExtensionId")] ProdPlanDetail instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                instance.Id = Guid.NewGuid();
                instance.OrgId = applicationUser.OrgId.ToSysGuid();
                instance.CreatedBy = applicationUser.Id;
                instance.CreatedDate = DateTime.UtcNow;
                instance.ModifiedBy = applicationUser.Id;
                instance.ModifiedDate = DateTime.UtcNow;
                instance.Active = true;
                instance.Name = instance.JobNo.ToString();

                instance.StartDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.StartDateTime, User.TimeZone());
                instance.EndDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.EndDateTime, User.TimeZone());
                instance.ActualStartDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.ActualStartDateTime, User.TimeZone());
                instance.ActualEndDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.ActualEndDateTime, User.TimeZone());

                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = instance.Id.ToString() });
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
                {
                    HandleDbEntityValidationException(dbEntityValidationException);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
                {
                    HandleDbUpdateException(dbUpdateException);
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }

                // If we got this far, something failed, redisplay form
                await SetupInstanceForm(applicationUser, instance);
                return View(instance);
            }

            return HttpNotFound();
        }

        // GET: /ProdPlanDetail/Edit/5
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ProdPlanDetail instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, ProdPlanDetail instance)
        {
            ViewBag.WeightUnitId =
                new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.WeightUnitId);
        }

        // POST: /ProdPlanDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProdPlanId,Serial,JobNo,Quantity,Weight,WeightUnitId,StartDateTime,EndDateTime,CycleTimeInHour,CycleCapacity,JobType,SetupTimeInHrs,AddOnTimeInHrs,ActualStartDateTime,ActualEndDateTime,Remark,Authorized,CustomColumn1,ColExtensionId,TimeStamp")] ProdPlanDetail instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                ProdPlanDetail model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  WorksAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("WorksAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;

                    }
                }
                //If user is not in role of WorksAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;

                model.Serial = instance.Serial;
                model.JobNo = instance.JobNo;
                model.Name = instance.JobNo.ToString();
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.JobType = instance.JobType;
                model.CycleTimeInHour = instance.CycleTimeInHour;
                model.CycleCapacity = instance.CycleCapacity;
                model.SetupTimeInHrs = instance.SetupTimeInHrs;
                model.AddOnTimeInHrs = instance.AddOnTimeInHrs;

                model.StartDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.StartDateTime, User.TimeZone());
                model.EndDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.EndDateTime, User.TimeZone());
                model.ActualStartDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.ActualStartDateTime, User.TimeZone());
                model.ActualEndDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.ActualEndDateTime, User.TimeZone());

                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;
                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "ProdPlanId", "Serial", "JobNo", "Quantity", "Weight", "WeightUnitId", "CycleTimeInHour", "CycleCapacity", "JobType", "SetupTimeInHrs", "AddOnTimeInHrs", "StartDateTime", "EndDateTime", "ActualStartDateTime", "ActualEndDateTime", "Remark", "Authorized", "IsRawBookingDone", "IsIssuedForProduction", "Active", "CustomColumn1", "ColExtensionId" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = instance.Id.ToString() });
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
                {
                    HandleDbEntityValidationException(dbEntityValidationException);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
                {
                    HandleDbUpdateException(dbUpdateException);
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }

                // If we got this far, something failed, redisplay form
                await SetupInstanceForm(applicationUser, instance);
                return View(instance);
            }

            return HttpNotFound();
        }


        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public JsonResult BookRawMaterial(string id, string jobNo)
        {
            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            var returnData = new  { Message = "Success"};
            try
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                ProdPlanRawBookingService.Create(id, jobNo, applicationUser.Id, applicationUser.OrgId.ToString());
                _unitOfWork.SaveChangesAsync();

                // Convert anonymous object to JSON response
                response = Json(returnData, JsonRequestBehavior.AllowGet);
                return response;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
            {
                HandleDbEntityValidationException(dbEntityValidationException);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
            {
                HandleDbUpdateException(dbUpdateException);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            returnData = new { Message = TempData["CustomErrorMessage"].ToString() };

            // Convert anonymous object to JSON response
            response = Json(returnData, JsonRequestBehavior.AllowGet);
            return response;
        }

        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public JsonResult IssueForProduction(string id, string jobNo)
        {
            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            var returnData = new { Message = "Success" };
            try
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                _service.IssueForProduction(id, jobNo, applicationUser.Id, applicationUser.OrgId.ToString());
                _unitOfWork.SaveChangesAsync();

                // Convert anonymous object to JSON response
                response = Json(returnData, JsonRequestBehavior.AllowGet);
                return response;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
            {
                HandleDbEntityValidationException(dbEntityValidationException);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
            {
                HandleDbUpdateException(dbUpdateException);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            returnData = new { Message = TempData["CustomErrorMessage"].ToString() };

            // Convert anonymous object to JSON response
            response = Json(returnData, JsonRequestBehavior.AllowGet);
            return response;
        }

        // GET: /ProdPlanDetail/Delete/5
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (id.ToString().IsNullOrWhiteSpace()) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            if (model == null) return HttpNotFound();

            model.AspNetUser = await AspNetUserService.FindAsync(model.CreatedBy); //Created
            model.AspNetUser1 = await AspNetUserService.FindAsync(model.ModifiedBy); // Modified
            model.WeightUnit = await _weightUnitService.FindAsync(model.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(model);
        }

        // POST: /ProdPlanDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                string parentGuid = string.Empty;
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    ProdPlanDetail model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    string logEntry = "Deleted ProdPlanDetail: Id=" + model.Id + " JobNo=" + model.JobNo + "  StartDate=" + model.StartDateTime;
                    _service.Delete(id.ToString());

                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, logEntry);
                }

                return RedirectToAction("Index");
            }

            return HttpNotFound();
        }

        /// <summary>
        /// This method is called to update address through ajax request
        /// </summary>
        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
        public virtual JsonResult PostBackJobNo(string stringifiedParam)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
            var param = stringifiedParam;

            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            //Get the balance quantity available for planning
            decimal quantityInPlanning = _service.QuantityInPlanning(param, applicationUser.Id, applicationUser.OrgId.ToString());
            var orderLineItem = OrderLineItemService.SelectSingle(param, applicationUser.Id, applicationUser.OrgId.ToString());
            decimal productionQuantity = orderLineItem.ProductionQuantity ?? 0;
            decimal quantityUnPlanned = productionQuantity - quantityInPlanning;

            // Now get cycle time, capacity of all operations for he prdouct and add all operation's cycle time.
            // For capacity figure, fetch capacities for all the operations and take the minimum capacity.
            Guid productId = orderLineItem.ProductId;
            decimal compositeCycleTimeInHour = 0;
            decimal minimumCycleCapacity = 0;

            minimumCycleCapacity = orderLineItem.Product.Operations.FirstOrDefault() == null ? 0 : orderLineItem.Product.Operations.FirstOrDefault().CycleCapacity;

            foreach (var operation in orderLineItem.Product.Operations)
            {
                compositeCycleTimeInHour += operation.CycleTimeInHour;

                if (operation.CycleCapacity < minimumCycleCapacity)
                {
                    minimumCycleCapacity = operation.CycleCapacity;
                }
            }

            //////// Create anonymous object
            ////var returnData = new
            ////{
            ////    Item = JsonConvert.SerializeObject(address)
            ////};

            // Create anonymous object
            var returnData = new
            {
                Quantity = quantityUnPlanned,
                Weight = orderLineItem.ProductionWeight,
                WeightUnitId = orderLineItem.ProductionWeightUnitId,
                CycleTimeInHour = compositeCycleTimeInHour,
                CycleCapacity = minimumCycleCapacity
            };

            // Convert anonymous object to JSON response
            response = Json(returnData, JsonRequestBehavior.AllowGet);

            // Return JSON
            return response;
        }

        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            decimal jobNoDecimal = -1;
            try
            {
                jobNoDecimal = searchParam.ToJobNumber();
            }
            catch (Exception exception)
            {
                HandleException(exception);
                return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"].ToString() });
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            
            var records = await _service.SelectAsyncLineItems(jobNoDecimal.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            var masterProductionScheduleViewModels = new List<MasterProductionScheduleViewModel>();

            if (records.Any())
            {
                records.ForEach(p =>
                {
                    //Get the balance quantity available for planning
                    decimal quantityInPlanning = _service.QuantityInPlanning(p.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    decimal quantityInProduction = _service.QuantityInProduction(p.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var orderLineItem = OrderLineItemService.SelectSingle(p.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    decimal productionQuantity = orderLineItem.ProductionQuantity ?? 0;
                    decimal quantityUnPlanned = productionQuantity - quantityInPlanning;

                    var viewModel = new MasterProductionScheduleViewModel
                    {
                        Id = p.Id,
                        StartDateTime = p.StartDateTime,
                        EndDateTime = p.EndDateTime,
                        Serial = p.Serial,
                        JobNo = p.JobNo,
                        Quantity = p.Quantity,
                        Weight = p.Weight,
                        WeightUnitId = p.WeightUnitId,
                        IsRawBookingDone = p.IsRawBookingDone,
                        IsIssuedForProduction = p.IsIssuedForProduction,
                        QuantityUnplanned = quantityUnPlanned,
                        QuantityInProduction = quantityInProduction
                    };

                    masterProductionScheduleViewModels.Add(viewModel);
                });
            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = masterProductionScheduleViewModels.ToPagedList(pageNumber, 50); // will only contain 50 products max because of the pageSize

            return View("Index", enumerablePage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Handle Exception
        private void HandleDbUpdateException(DbUpdateException dbUpdateException)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbUpdateException));
            string innerExceptionMessage = dbUpdateException.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = dbUpdateException.InnerException.Message;

            if (innerExceptionMessage.Contains("UQ") ||
                innerExceptionMessage.Contains("Primary Key") ||
                innerExceptionMessage.Contains("PK"))
            {
                if (innerExceptionMessage.Contains("PriceBookId_ItemId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Item. Item already exists.";
                    AddErrors("Duplicate Item. Item already exists.");
                }
                else
                {
                    TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exists.";
                    AddErrors("Duplicate Name or Code. Key record already exists.");
                }
            }
            else if (innerExceptionMessage.Contains("_CheckQuantity"))
            {
                TempData["CustomErrorMessage"] = "Invalid quantity.";
                AddErrors("Invalid quantity.");
            }
            else if (innerExceptionMessage.Contains("JobNo"))
            {
                AddErrors("Invalid Job No.");
                TempData["CustomErrorMessage"] = "Invalid Job No.";

            }
            else if (innerExceptionMessage.Contains("CK_ProdPlan_CheckProdPlanProcess"))
            {
                AddErrors("Process cannot be changed. Not matching with master planning.");
                TempData["CustomErrorMessage"] = "Invalid Job No.";
            }
            else
            {
                AddErrors(dbUpdateException.Message.ToString() + " ___" + DateTime.UtcNow);
                TempData["CustomErrorMessage"] = dbUpdateException.Message.ToString() + " - " + dbUpdateException.InnerException.Message.ToString();

            }
        }

        private string HandleDbEntityValidationException(DbEntityValidationException dbEntityValidationException)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbEntityValidationException));
            // Retrieve the error messages as a list of strings.
            var errorMessages = dbEntityValidationException.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(dbEntityValidationException.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = dbEntityValidationException.InnerException.Message;
            AddErrors(exceptionMessage + " ___" + DateTime.UtcNow);

            return exceptionMessage;
        }

        private void HandleException(Exception exception)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            TempData["CustomErrorMessage"] = exception.Message;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;
            AddErrors(exception.Message.ToString() + " ___" + DateTime.UtcNow);
        }

        private void AddErrors(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
        }

        #endregion Handle Exception
    }
}
