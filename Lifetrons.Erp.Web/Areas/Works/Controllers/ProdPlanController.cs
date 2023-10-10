//using System.Collections.Generic;
//using System.Data.Entity.Infrastructure;
//using System.Data.Entity.Validation;
//using System.Linq;
//using System.Web.WebPages;
//using Lifetrons.Erp.Data;
//using Lifetrons.Erp.Helpers;
//using Lifetrons.Erp.Models;
//using Lifetrons.Erp.Service;
//using Microsoft.AspNet.Identity;
//using Microsoft.Practices.Unity;
//using PagedList;
//using Repository;
//using System;
//using System.Net;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//using Repository.Pattern.Infrastructure;
//using Repository.Pattern.UnitOfWork;
//using WebGrease.Css.Extensions;
//using Task = System.Threading.Tasks.Task;

//namespace Lifetrons.Erp.Works.Controllers
//{
//    [Authorize]
//    public class ProdPlanController : BaseController
//    {
//        [Dependency]
//        public IOperationService OperationService { get; set; }

//        [Dependency]
//        public IProcessService ProcessService { get; set; }

//        [Dependency]
//        public IProdPlanDetailService ProdPlanDetailService { get; set; }

//        private readonly IProdPlanService _service;
//        private readonly IUnitOfWorkAsync _unitOfWork;
//        private readonly IAspNetUserService _aspNetUserService;

//        public ProdPlanController(IProdPlanService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
//        {
//            try
//            {
//                _service = service;
//                _unitOfWork = unitOfWork;
//                _aspNetUserService = aspNetUserService;
//            }
//            catch (Exception ex)
//            {
                
//                throw ex;
//            }
           
//        }

//        // GET: /ProdPlan/
//        public async Task<ActionResult> Index(int? page)
//        {
//            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

//            var records =  await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
//            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
//            var enumerablePage = records.OrderByDescending(x => x.StartDateTime).ToPagedList(pageNumber, 50); // will only contain 25 products max because of the pageSize

//            return View(enumerablePage);
//        }

//        // GET: /ProdPlan/Details/5
//        public async Task<ActionResult> Details(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

//            ProdPlan instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
//            if (instance == null)
//            {
//                return HttpNotFound();
//            }

//            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
//            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            
//            //instance.Process = await ProcessService.FindAsync(instance.ProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
//            instance.ProdPlanDetails = (await ProdPlanDetailService.SelectAsyncLineItems(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).ToList();

//            //Set id to pass on detail partial page
//            ViewBag.ProdPlanId = instance.Id;

//            return View(instance);
//        }

//        // GET: /ProdPlan/Create
//        [EsAuthorize(Roles = "canEdit, WorksAuthorize")]
//        public async Task<ActionResult> Create()
//        {
//            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
//            //ViewBag.ProcessId = new SelectList(await ProcessService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
//            ViewBag.ProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Production"].ToSysGuid();
//            return View();
//        }

//        // POST: /ProdPlan/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [EsAuthorize(Roles = "canEdit, WorksAuthorize")]
//        public async Task<ActionResult> Create([Bind(Include = "ProcessId,CycleTimeInHour,CycleCapacity,QuantityPerHour,StartDateTime,EndDateTime,RefNo,JobType,SetupTimeInHrs,AddOnTimeInHrs,Remark,Authorized,CustomColumn1,ColExtensionId")] ProdPlan instance)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

//                instance.Id = Guid.NewGuid();
//                instance.OrgId = applicationUser.OrgId.ToSysGuid();
//                instance.CreatedBy = applicationUser.Id;
//                instance.CreatedDate = DateTime.UtcNow;
//                instance.ModifiedBy = applicationUser.Id;
//                instance.ModifiedDate = DateTime.UtcNow;
//                instance.Active = true;
//                instance.ProcessId = instance.ProcessId.ToGuid();
//                instance.StartDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.StartDateTime, User.TimeZone());
//                instance.EndDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.EndDateTime, User.TimeZone());
//                instance.ProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Production"].ToSysGuid();
//                try
//                {
//                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
//                    await _unitOfWork.SaveChangesAsync();
//                    return RedirectToAction("Details", new { id = instance.Id });
//                }
//                catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
//                {
//                    HandleDbEntityValidationException(dbEntityValidationException);
//                }
//                catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
//                {
//                    HandleDbUpdateException(dbUpdateException);
//                }
//                catch (Exception exception)
//                {
//                    HandleException(exception);
//                }

//                // If we got this far, something failed, redisplay form
//                await SetupInstanceForm(applicationUser, instance);
//                return View(instance);
//            }

//            return HttpNotFound();
//        }

//        // GET: /ProdPlan/Edit/5
//        [EsAuthorize(Roles = "canEdit, WorksAuthorize")]
//        public async Task<ActionResult> Edit(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
//            ProdPlan instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
//            if (instance == null)
//            {
//                return HttpNotFound();
//            }

//            //If instance.Authorized, only user with "WorksAuthorize" role is allowed to edit the record
//            if (instance.Authorized && !User.IsInRole("WorksAuthorize"))
//            {
//                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
//                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
//                throw exception;
//            }

//            await SetupInstanceForm(applicationUser, instance);
            
//            return View(instance);
//        }

//        private async System.Threading.Tasks.Task SetupInstanceForm(ApplicationUser applicationUser, ProdPlan instance)
//        {
//            //ViewBag.ProcessId =
//            //    new SelectList(await ProcessService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
//            //        "Name", instance.ProcessId);
//            ViewBag.ProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Production"].ToSysGuid();
//        }

//        // POST: /ProdPlan/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [EsAuthorize(Roles = "canEdit, WorksAuthorize")]
//        [Audit(AuditingLevel = 1)]
//        public async Task<ActionResult> Edit([Bind(Include = "Id,ProcessId,CycleTimeInHour,CycleCapacity,StartDateTime,EndDateTime,RefNo,JobType,SetupTimeInHrs,AddOnTimeInHrs,Remark,Authorized,CustomColumn1,ColExtensionId,TimeStamp")] ProdPlan instance)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
//                ProdPlan model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

//                //If user is in role of  WorksAuthorize role, process editing. Also update status in long desc field.
//                if (User.IsInRole("WorksAuthorize"))
//                {
//                    if (model.Authorized != instance.Authorized)
//                    {
//                        model.Authorized = instance.Authorized;
//                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
//                    }
//                }
//                //If user is not in role of WorksAuthorize and record is Authorized, stop editng and show appropriate message.
//                else if (model.Authorized)
//                {
//                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
//                                                     "You should have authorization rights to authorize/unauthorize records.";
//                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
//                }

//                model.ObjectState = ObjectState.Modified;
//                model.ModifiedBy = applicationUser.Id;
//                model.ModifiedDate = DateTime.UtcNow;
//                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
//                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;
                
//                //model.ProcessId = instance.ProcessId.ToGuid();
//                model.ProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();
//                model.RefNo = instance.RefNo;
//                model.JobType = instance.JobType;
//                model.StartDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.StartDateTime, User.TimeZone());
//                model.EndDateTime = ControllerHelper.ConvertDateTimeToUtc(instance.EndDateTime, User.TimeZone());
//                model.CycleTimeInHour = instance.CycleTimeInHour;
//                model.CycleCapacity = instance.CycleCapacity;
//                model.SetupTimeInHrs = instance.SetupTimeInHrs;
//                model.AddOnTimeInHrs = instance.AddOnTimeInHrs;

//                ModelState.Clear();
//                try
//                {
//                    if (TryValidateModel(model))
//                    {
//                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
//                        await _unitOfWork.SaveChangesAsync();

//                        // Object Comparison and logging in audit
//                        var membersToCompare = new List<string>() { "ProcessId", "CycleTimeInHour", "CycleCapacity", "StartDateTime", "EndDateTime", "RefNo", "JobType", "SetupTimeInHrs", "AddOnTimeInHrs", "Remark", "Authorized", "CustomColumn1", "ColExtensionId" };
//                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
//                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

//                        return RedirectToAction("Details", new { id = model.Id });
//                    }
//                }
//                catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
//                {
//                    HandleDbEntityValidationException(dbEntityValidationException);
//                }
//                catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
//                {
//                    HandleDbUpdateException(dbUpdateException);
//                }
//                catch (Exception exception)
//                {
//                    HandleException(exception);
//                }

//                // If we got this far, something failed, redisplay form
//                await SetupInstanceForm(applicationUser, instance);
//                return View(instance);
//            }

//            return HttpNotFound();
//        }

//        // GET: /ProdPlan/Delete/5
//        [EsAuthorize(Roles = "WorksAuthorize")]
//        [Audit(AuditingLevel = 0)]
//        public async Task<ActionResult> Delete(Guid? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
//            ProdPlan model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
//            if (model == null)
//            {
//                return HttpNotFound();
//            }

//            model.AspNetUser = ControllerHelper.GetAspNetUser(model.CreatedBy); //Created
//            model.AspNetUser1 = ControllerHelper.GetAspNetUser(model.ModifiedBy); // Modified

//            model.Process = await ProcessService.FindAsync(model.ProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
//            model.ProdPlanDetails = (await ProdPlanDetailService.SelectAsyncLineItems(model.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).ToList();

//            ViewBag.Id = model.Id;

//            return View(model);
//        }

//        // POST: /ProdPlan/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        [EsAuthorize(Roles = "WorksAuthorize")]
//        [Audit(AuditingLevel = 1)]
//        public async Task<ActionResult> DeleteConfirmed(Guid id)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
//                if (applicationUser.OrgId != null) //Only approver is allowed
//                {
//                    ProdPlan model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
//                    var lineItems = ProdPlanDetailService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

//                    string logEntry = "Deleted ProdPlan: Id=" + id + " ProcessId = " + model.ProcessId + " Start Date=" + model.StartDateTime + " End Date=" + model.EndDateTime  + " RefNo=" + model.RefNo;
//                    lineItems.ForEach(p => logEntry += "\n LineItems: Id=" + p.Id + " ProdPlanId=" + p.ProdPlanId + " JobNo=" + p.JobNo + " Start Date=" + model.StartDateTime + " End Date=" + model.EndDateTime + " Quantity=" + p.Quantity);
                    
//                    _service.Delete(model);
//                    await _unitOfWork.SaveChangesAsync();

//                    // Logging in audit
//                    HttpContext.Items.Add(ControllerHelper.AuditData, logEntry);
//                }
//                return RedirectToAction("Index");
//            }
//            return HttpNotFound();
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                _unitOfWork.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        public async Task<ActionResult> Search(string searchParam, int? page)
//        {
//            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

//            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
//            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
//            var enumerablePage =
//                 records.Where(x => x.RefNo.ToLower().Contains(searchParam.ToLower()) || x.RefNo.ToLower().Contains(searchParam.ToLower()))
//                    .OrderByDescending(x => x.ModifiedDate)
//                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

//            return View("Index", enumerablePage);
//        }

//        /// <summary>
//        /// This method is called to update address through ajax request
//        /// </summary>
//        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
//        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
//        public virtual JsonResult PostBackProcess(string stringifiedParam)
//        {
//            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

//            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
//            var param = stringifiedParam;

//            var response = new JsonResult();
//            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

//            var process = ProcessService.Find(param, applicationUser.Id, applicationUser.OrgId.ToString());

//            //Calculate consolidated time for all the processes involved 
//            //OperationService.SelectLineItemsAsync()

//            // Create anonymous object
//            var returnData = new
//            {
//                CycleTimeInHour = process.CycleTimeInHour,
//                CycleCapacity = process.CycleCapacity,
//            };

//            // Convert anonymous object to JSON response
//            response = Json(returnData, JsonRequestBehavior.AllowGet);

//            // Return JSON
//            return response;
//        }

//        #region Handle Exception
//        private void HandleDbUpdateException(DbUpdateException dbUpdateException)
//        {
//            //Log the error
//            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbUpdateException));
//            string innerExceptionMessage = dbUpdateException.InnerException.InnerException.Message;
//            TempData["CustomErrorMessage"] = innerExceptionMessage;
//            TempData["CustomErrorDetail"] = dbUpdateException.InnerException.Message;

//            if (innerExceptionMessage.Contains("UQ") ||
//                innerExceptionMessage.Contains("Primary Key") ||
//                innerExceptionMessage.Contains("PK"))
//            {
//                if (innerExceptionMessage.Contains("PriceBookId_ItemId"))
//                {
//                    TempData["CustomErrorMessage"] = "Duplicate Item. Item already exists.";
//                    AddErrors("Duplicate Item. Item already exists.");
//                }
//                else
//                {
//                    TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exists.";
//                    AddErrors("Duplicate Name or Code. Key record already exists.");
//                }
//            }
//            else if (innerExceptionMessage.Contains("_CheckQuantity"))
//            {
//                TempData["CustomErrorMessage"] = "Invalid quantity.";
//                AddErrors("Invalid quantity.");
//            }
//            else if (innerExceptionMessage.Contains("JobNo"))
//            {
//                AddErrors("Invalid Job No.");
//                TempData["CustomErrorMessage"] = "Invalid Job No.";

//            }
//            else if (innerExceptionMessage.Contains("CK_ProdPlan_CheckProdPlanProcess"))
//            {
//                AddErrors("Process cannot be changed for this plan. Already scheduled jobs.");
//                TempData["CustomErrorMessage"] = "Invalid Job No.";
//            }
//            else
//            {
//                AddErrors(dbUpdateException.Message.ToString() + " - " + DateTime.UtcNow);
//                TempData["CustomErrorMessage"] = dbUpdateException.Message.ToString() + " - " +
//                                                 dbUpdateException.InnerException.Message.ToString();

//            }
//        }

//        private string HandleDbEntityValidationException(DbEntityValidationException dbEntityValidationException)
//        {
//            //Log the error
//            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbEntityValidationException));
//            // Retrieve the error messages as a list of strings.
//            var errorMessages = dbEntityValidationException.EntityValidationErrors
//                .SelectMany(x => x.ValidationErrors)
//                .Select(x => x.ErrorMessage);

//            // Join the list to a single string.
//            var fullErrorMessage = string.Join("; ", errorMessages);

//            // Combine the original exception message with the new one.
//            var exceptionMessage = string.Concat(dbEntityValidationException.Message, " The validation errors are: ", fullErrorMessage);

//            // Throw a new DbEntityValidationException with the improved exception message.
//            TempData["CustomErrorMessage"] = exceptionMessage;
//            TempData["CustomErrorDetail"] = dbEntityValidationException.InnerException.Message;
//            AddErrors(exceptionMessage);

//            return exceptionMessage;
//        }

//        private void HandleException(Exception exception)
//        {
//            //Log the error
//            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
//            TempData["CustomErrorMessage"] = exception.Message;
//            TempData["CustomErrorDetail"] = exception.InnerException.Message;
//            AddErrors(exception.Message.ToString() + " - " + DateTime.UtcNow);
//        }

//        private void AddErrors(string errorMessage)
//        {
//            ModelState.AddModelError("", errorMessage);
//        }

//        #endregion Handle Exception
//    }
//}
