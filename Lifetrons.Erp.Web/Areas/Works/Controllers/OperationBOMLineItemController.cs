using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class OperationBOMLineItemController : BaseController
    {
        [Dependency]
        public IWeightUnitService WeightUnitService { get; set; }

        [Dependency]
        public IOperationService OperationService { get; set; }

        [Dependency]
        public IEnterpriseStageService EnterpriseStageService { get; set; }

        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IProductService ProductService { get; set; }

        [Dependency]
        public IItemService ItemService { get; set; }

        [Dependency]
        public IBOMLineItemService BOMLineItemService { get; set; }

        [Dependency]
        public IEmailConfigService EmailConfigService { get; set; }

        private readonly IOperationBOMLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public OperationBOMLineItemController(IOperationBOMLineItemService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: OperationBOMLineItem
        public ActionResult Index(string productId, string enterpriseStageId, string processId, string operationId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(productId, enterpriseStageId, processId, applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.operationId = operationId;

            ViewBag.ProductId = productId;
            ViewBag.ProductName = (ProductService.Find(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.EnterpriseStageId = enterpriseStageId;
            ViewBag.EnterpriseStageName = (EnterpriseStageService.Find(enterpriseStageId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProcessId = processId;
            ViewBag.ProcessName = (ProcessService.Find(processId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return PartialView(model);
        }

        // GET: OperationBOMLineItem/Details/5
        public async Task<ActionResult> Details(string productId, string enterpriseStageId, string processId, string itemId)
        {
            if (productId == null || enterpriseStageId == null || processId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            OperationBOMLineItem instance = await _service.FindAsync(productId, enterpriseStageId, processId, itemId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = await _aspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await _aspNetUserService.FindAsync(instance.ModifiedBy); // Modified

            ViewBag.ProductId = productId;
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.EnterpriseStageId = enterpriseStageId;
            ViewBag.EnterpriseStageName = (await EnterpriseStageService.FindAsync(enterpriseStageId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProcessId = processId;
            ViewBag.ProcessName = (await ProcessService.FindAsync(processId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ItemId = itemId;
            ViewBag.ItemName = (await ItemService.FindAsync(itemId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            instance.WeightUnit = await WeightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.WeightUnit1 = await WeightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(instance);
        }

        // GET: OperationBOMLineItem/Create
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Create(string productId, string enterpriseStageId, string processId, string operationId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            
            ViewBag.operationId = operationId;

            ViewBag.ProductId = productId;
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.EnterpriseStageId = enterpriseStageId;
            ViewBag.EnterpriseStageName = (await EnterpriseStageService.FindAsync(enterpriseStageId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProcessId = processId;
            ViewBag.ProcessName = (await ProcessService.FindAsync(processId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            List<Guid> bomItems = BOMLineItemService.SelectLineItems(productId, applicationUser.Id, applicationUser.OrgId.ToString()).Select(s => s.ItemId).ToList();
            ViewBag.ItemId = new SelectList(await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString(), bomItems), "Id", "Name");

            ViewBag.WeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.AllowedLossWeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: OperationBOMLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Create([Bind(Include = "OperationId,ProductId,EnterpriseStageId,ProcessId,ItemId,Serial,ShrtDesc,Desc,Quantity,Weight,WeightUnitId,AllowedLossQuantity,AllowedLossWeight,AllowedLossWeightUnitId,Authorized")] OperationBOMLineItem instance)
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
                //In Create - UnAuthourize instance if used do not has the Authorize role.
                if (instance.Authorized && !User.IsInRole("WorksAuthorize"))
                {
                    instance.Authorized = false;
                }

                instance.Desc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                instance.ProductId = instance.ProductId.ToGuid();
                instance.EnterpriseStageId = instance.EnterpriseStageId.ToGuid();
                instance.ProcessId = instance.ProcessId.ToGuid();
                instance.ItemId = instance.ItemId.ToGuid();

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

            return RedirectToAction("Details", new { productId = instance.ProductId, enterpriseStageId = instance.EnterpriseStageId, processId = instance.ProcessId, itemId = instance.ItemId });
        }

        // GET: OperationBOMLineItem/Edit/5
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Edit(string productId, string enterpriseStageId, string processId, string itemId)
        {
            if (productId == null || enterpriseStageId == null || processId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            OperationBOMLineItem instance = await _service.FindAsync(productId, enterpriseStageId, processId, itemId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "WorksAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("WorksAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.ProductId = productId;
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.EnterpriseStageId = enterpriseStageId;
            ViewBag.EnterpriseStageName = (await EnterpriseStageService.FindAsync(enterpriseStageId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProcessId = processId;
            ViewBag.ProcessName = (await ProcessService.FindAsync(processId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            ViewBag.ItemId = itemId;
            ViewBag.ItemName = (await ItemService.FindAsync(itemId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            ViewBag.WeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);
            ViewBag.AllowedLossWeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AllowedLossWeightUnitId);

            return View(instance);
        }

        // POST: OperationBOMLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OperationId,ProductId,EnterpriseStageId,ProcessId,ItemId,Serial,ShrtDesc,Desc,Quantity,Weight,WeightUnitId,AllowedLossQuantity,AllowedLossWeight,AllowedLossWeightUnitId,Authorized,TimeStamp")] OperationBOMLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                OperationBOMLineItem model = await _service.FindAsync(instance.ProductId.ToString(), instance.EnterpriseStageId.ToString(), instance.ProcessId.ToString(), instance.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  WorksAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("WorksAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
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

                model.ShrtDesc = instance.ShrtDesc;
                string instanceDesc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                model.Desc = model.Desc.IsEmpty() ? instanceDesc : model.Desc + "\n" + instanceDesc;

                model.ItemId = instance.ItemId.ToGuid();
                model.Serial = instance.Serial;
                model.ShrtDesc = instance.ShrtDesc;
                model.Serial = instance.Serial;
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.AllowedLossQuantity = instance.AllowedLossQuantity;
                model.AllowedLossWeight = instance.AllowedLossWeight;
                model.AllowedLossWeightUnitId = instance.AllowedLossWeightUnitId;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "ItemId", "ShrtDesc", "Desc", "Quantity", "Weight", "WeightUnitId", "AllowedLossQuantity", "AllowedLossWeight", "AllowedLossWeightUnitId", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { productId = instance.ProductId, enterpriseStageId = instance.EnterpriseStageId, processId = instance.ProcessId, itemId = instance.ItemId });
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

        // GET: OperationBOMLineItem/Delete/5
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(string productId, string enterpriseStageId, string processId, string itemId)
        {
            if (productId == null || enterpriseStageId == null || processId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            OperationBOMLineItem model = await _service.FindAsync(productId, enterpriseStageId, processId, itemId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = productId;
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.EnterpriseStageId = enterpriseStageId;
            ViewBag.EnterpriseStageName = (await EnterpriseStageService.FindAsync(enterpriseStageId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProcessId = processId;
            ViewBag.ProcessName = (await ProcessService.FindAsync(processId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            ViewBag.ItemId = itemId;
            ViewBag.ItemName = (await ItemService.FindAsync(itemId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            ViewBag.WeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", model.WeightUnitId);
            ViewBag.AllowedLossWeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", model.AllowedLossWeightUnitId);

            return View(model);
        }


        // POST: OperationBOMLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(string productId, string enterpriseStageId, string processId, string itemId)
        {
            if (productId == null || enterpriseStageId == null || processId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    OperationBOMLineItem model = await _service.FindAsync(productId, enterpriseStageId, processId, itemId, applicationUser.Id, applicationUser.OrgId.ToString());
                    var auditLogMessage = "Deleted: OperationBOMLineItem For Product = " + model.ProductId +
                                          "Process = " + model.ProcessId + " Enterprise Stage=" +
                                          model.EnterpriseStageId + " Item =" + model.ItemId + " OperationId=" +
                                          model.OperationId;

                    _service.Delete(model); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, auditLogMessage);
                }

                return RedirectToAction("Details", "Operation", new { productId = productId, enterpriseStageId = enterpriseStageId, processId = processId });
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
