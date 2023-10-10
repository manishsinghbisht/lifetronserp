using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class ProcessController : BaseController
    {
        private readonly IProcessService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        [Dependency]
        public IEnterpriseStageService EnterpriseStageService { get; set; }

        public ProcessController(IProcessService service, IUnitOfWorkAsync unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        // GET: Process
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 50); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: Process/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Process instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy);
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy);
            instance.EnterpriseStage = await EnterpriseStageService.FindAsync(instance.EnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(instance);
        }

        // GET: Process/Create
         [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.EnterpriseStageId = new SelectList(await EnterpriseStageService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            return View();
        }

        // POST: Process/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
         public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,EnterpriseStageId,Authorized,Serial,LabourRatePerHour,DepreciationRatePerHour,EnergyRatePerHour,OverheadRatePerHour,OtherDirectExpensesRatePerHour,OtherInDirectExpensesRatePerHour,CycleTimeInHour,CycleCapacity,Remark")] Process instance)
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
                instance.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                if (instance.QuantityPerHour == 0)
                {
                    instance.QuantityPerHour = 1;
                }
                if (instance.CycleTimeInHour == 0)
                {
                    instance.CycleTimeInHour = (Decimal)0.0166667; //1 minute = 0.0166667 hour
                }
                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var exceptionMessage = HandleDbEntityValidationException(ex);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    HandleDbUpdateException(exception);
                    throw;
                }
            }

            return RedirectToAction("Details", new { id = instance.Id });
        }

        // GET: Process/Edit/5
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Process instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            ViewBag.EnterpriseStageId = new SelectList(await EnterpriseStageService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.EnterpriseStageId);

            return View(instance);
        }

        // POST: Process/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,EnterpriseStageId,Authorized,Serial,LabourRatePerHour,DepreciationRatePerHour,EnergyRatePerHour,OverheadRatePerHour,OtherDirectExpensesRatePerHour,OtherInDirectExpensesRatePerHour,CycleTimeInHour,CycleCapacity,Remark")] Process instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Process model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                model.Authorized = instance.Authorized;
                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;

                model.ShrtDesc = instance.ShrtDesc;
                model.EnterpriseStageId = instance.EnterpriseStageId;
                model.Serial = instance.Serial;
                model.LabourRatePerHour = instance.LabourRatePerHour;
                model.DepreciationRatePerHour=instance.DepreciationRatePerHour;
                model.EnergyRatePerHour = instance.EnergyRatePerHour;
                model.OverheadRatePerHour = instance.OverheadRatePerHour;
                model.OtherDirectExpensesRatePerHour = instance.OtherDirectExpensesRatePerHour;
                model.OtherInDirectExpensesRatePerHour = instance.OtherInDirectExpensesRatePerHour;
                model.QuantityPerHour = instance.QuantityPerHour;
                model.CycleTimeInHour = instance.CycleTimeInHour;
                model.CycleCapacity = instance.CycleCapacity;

               ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "EnterpriseStageId", "Authorized", "Serial", "LabourRatePerHour", "DepreciationRatePerHour", "EnergyRatePerHour", "OverheadRatePerHour", "OtherDirectExpensesRatePerHour", "OtherInDirectExpensesRatePerHour", "QuantityPerHour", "ProcessTimeInHour", "Remark" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = model.Id });
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var exceptionMessage = HandleDbEntityValidationException(ex);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    HandleDbUpdateException(exception);
                    throw;
                }
                return View(instance);
            }

            return HttpNotFound();
        }

        // GET: Process/Delete/5
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Process model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Process/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Process model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: ProductId=" + id + " Name=" + model.Name + " Code=" + model.Code);
                }
                return RedirectToAction("Index");
            }

            return HttpNotFound();
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
        private void HandleDbUpdateException(DbUpdateException exception)
        {
            string innerExceptionMessage = exception.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;

            if (innerExceptionMessage.Contains("UQ") ||
                innerExceptionMessage.Contains("Primary Key") ||
                innerExceptionMessage.Contains("PK"))
            {
                if (innerExceptionMessage.Contains("PriceBookId_ProductId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Product. Product already exists.";
                }
                else
                {
                    TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exists.";
                }
            }
        }

        private string HandleDbEntityValidationException(DbEntityValidationException ex)
        {
            // Retrieve the error messages as a list of strings.
            var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = ex.GetType();
            return exceptionMessage;
        }

        #endregion Handle Exception
    }
}
