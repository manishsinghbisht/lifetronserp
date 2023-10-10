using System;
using System.Collections;
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
using System.Web.Routing;
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

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class OperationController : BaseController
    {
        [Dependency]
        public IEnterpriseStageService EnterpriseStageService { get; set; }

        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IProductService ProductService { get; set; }

        [Dependency]
        public IEmailConfigService EmailConfigService { get; set; }

        private readonly IOperationService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public OperationController(IOperationService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        [HttpPost]
        // Post: Operation
        public async Task<ActionResult> Index(string productIdParam, bool isAuthenticated)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            //Dropdown for changing product
            ViewBag.ProductIdParam = new SelectList(await ProductService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", productIdParam);

            if (productIdParam == null)
            {
                return View();
            }

            var model = await _service.SelectLineItemsAsync(productIdParam, applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.ProductId = productIdParam;
            ViewBag.ProductName = (await ProductService.FindAsync(productIdParam, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(model);
        }

        [HttpGet]
        // GET: Operation
        public async Task<ActionResult> Index(string productId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            //Dropdown for changing product
            ViewBag.ProductIdParam = new SelectList(await ProductService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", productId);

            if (String.IsNullOrEmpty(productId))
            {
                return View();
            }

            var model = await _service.SelectLineItemsAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.ProductId = productId;
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            //Dropdown for changing product
            ViewBag.ProductIdParam = new SelectList(await ProductService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", productId);

            return View(model);
        }

        // GET: Operation/Details/5
        public async Task<ActionResult> Details(string productId, string enterpriseStageId, string processId)
        {
            if (productId == null || enterpriseStageId == null || processId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Operation instance = await _service.FindAsync(productId, enterpriseStageId, processId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }


            instance.AspNetUser = await _aspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await _aspNetUserService.FindAsync(instance.ModifiedBy); // Modified
            instance.Product = await ProductService.FindAsync(instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.EnterpriseStage = await EnterpriseStageService.FindAsync(instance.EnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Process = await ProcessService.FindAsync(instance.ProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(instance);
        }

        // GET: Operation/Create
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Create(string productId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.ProductId = productId;
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.EnterpriseStageId = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Production"].ToSysGuid();
            ViewBag.ProcessId = new SelectList(await ProcessService.SelectEnterpriseStageProcessAsync(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Production"], applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: Operation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Create([Bind(Include = "ProductId,EnterpriseStageId,ProcessId,Serial,ShrtDesc,LabourRatePerHour,DepreciationRatePerHour,EnergyRatePerHour,OverheadRatePerHour,OtherDirectExpensesRatePerHour,OtherInDirectExpensesRatePerHour,CycleTimeInHour,CycleCapacity,Remark,Authorized")] Operation instance)
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

                instance.Remark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                instance.ProductId = instance.ProductId.ToGuid();
                instance.EnterpriseStageId = instance.EnterpriseStageId.ToGuid();
                instance.ProcessId = instance.ProcessId.ToGuid();

                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                    await SendUpdateEmail(instance);
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

            return RedirectToAction("Details", new { productId = instance.ProductId, enterpriseStageId = instance.EnterpriseStageId, processId = instance.ProcessId });
        }

        // GET: Operation/Edit/5
        [EsAuthorize(Roles = "WorksPlanner")]
        public async Task<ActionResult> Edit(string productId, string enterpriseStageId, string processId)
        {
            if (productId == null || enterpriseStageId == null || processId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Operation instance = await _service.FindAsync(productId, enterpriseStageId, processId, applicationUser.Id, applicationUser.OrgId.ToString());
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
            ViewBag.EnterpriseStageId = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Production"].ToSysGuid();
            ViewBag.ProcessId = new SelectList(await ProcessService.SelectEnterpriseStageProcessAsync(instance.EnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ProcessId);

            return View(instance);
        }

        // POST: Operation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProductId,EnterpriseStageId,ProcessId,Serial,ShrtDesc,LabourRatePerHour,DepreciationRatePerHour,EnergyRatePerHour,OverheadRatePerHour,OtherDirectExpensesRatePerHour,OtherInDirectExpensesRatePerHour,CycleTimeInHour,CycleCapacity,Remark,Authorized,TimeStamp")] Operation instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                bool sendUpdateMail = false;
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Operation model = await _service.FindAsync(instance.ProductId.ToString(), instance.EnterpriseStageId.ToString(), instance.ProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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

                //Check if update email need to be sent.
                sendUpdateMail = true;

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Authorized = instance.Authorized;

                model.ShrtDesc = instance.ShrtDesc;
                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;

                model.ProductId = instance.ProductId.ToGuid();
                model.EnterpriseStageId = instance.EnterpriseStageId.ToGuid();
                model.ProcessId = instance.ProcessId.ToGuid();

                model.Serial = instance.Serial;
                model.ShrtDesc = instance.ShrtDesc;
                model.LabourRatePerHour = instance.LabourRatePerHour;
                model.DepreciationRatePerHour = instance.DepreciationRatePerHour;
                model.EnergyRatePerHour = instance.EnergyRatePerHour;
                model.OverheadRatePerHour = model.OverheadRatePerHour;
                model.OtherDirectExpensesRatePerHour = instance.OtherDirectExpensesRatePerHour;
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
                        var membersToCompare = new List<string>() { "Serial", "ShrtDesc", "LabourRatePerHour", "DepreciationRatePerHour", "EnergyRatePerHour", "OverheadRatePerHour", "OtherDirectExpensesRatePerHour", "OtherInDirectExpensesRatePerHour", "QuantityPerHour", "CycleTimeInHour", "Remark", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        //Send update mail
                        if (sendUpdateMail) await SendUpdateEmail(instance);

                        return RedirectToAction("Details", new { productId = instance.ProductId, enterpriseStageId = instance.EnterpriseStageId, processId = instance.ProcessId });
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

        // GET: Operation/Delete/5
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(string productId, string enterpriseStageId, string processId)
        {
            if (productId == null || enterpriseStageId == null || processId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Operation model = await _service.FindAsync(productId, enterpriseStageId, processId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Operation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(string productId, string enterpriseStageId, string processId)
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
                    Operation model = await _service.FindAsync(productId, enterpriseStageId, processId, applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    var auditLogMessage = "Deleted: Operation For Product = " + model.Product.Name + "Process = " +
                                          model.Process.Name + " Enterprise Stage=" + model.EnterpriseStage.Name;

                    _service.Delete(model); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, auditLogMessage);
                }
                return RedirectToAction("Index", new { productId = productId });
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

        private async Task<bool> SendUpdateEmail(Operation instance)
        {
            try
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                string toAddress = applicationUser.AuthenticatedEmail;
                string ccAddress = applicationUser.AuthenticatedEmail;
                string subject = "Operation Updated - \"" + instance.Process.Name + " - " + instance.Product.Name.Substring(0, instance.Product.Name.Length > 8 ? 8 : instance.Product.Name.Length) + "\" !";
                string message = "You may want to look into the Operation - \"" + instance.Process.Name + " - " + instance.Product.Name + "\" " +
                    "assigned to you by " + applicationUser.FirstName + " " + applicationUser.LastName;

                await EmailConfigService.SendMail(applicationUser.Id, applicationUser.OrgId.ToString(), toAddress, ccAddress, subject, message, true);
            }
            catch (Exception exception)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            }
            return true;
        }

        /// <summary>
        /// This method is called to update dropdown through ajax request
        /// </summary>
        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
        public async virtual Task<JsonResult> ProcessJsonResponseForProcessDdl(string stringifiedParam)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
            var param = stringifiedParam;

            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var select = new SelectList(await ProcessService.SelectEnterpriseStageProcessAsync(stringifiedParam, applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            // Create anonymous object
            //var returnData = new
            //{
            //    Item = JsonConvert.SerializeObject(select)
            //};

            // Convert anonymous object to JSON response
            response = Json(select, JsonRequestBehavior.AllowGet);

            // Return JSON
            return response;
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
