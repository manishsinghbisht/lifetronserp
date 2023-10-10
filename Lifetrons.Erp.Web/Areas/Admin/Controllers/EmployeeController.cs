using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Admin.Controllers
{
    [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit, HRAdminView")]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        [Dependency]
        public IContactService ContactService { get; set; }

        [Dependency]
        public IDepartmentService DepartmentService { get; set; }

       public EmployeeController(IEmployeeService service, IUnitOfWorkAsync unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        // GET: Employee
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: Employee/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Employee instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            
            instance.Contact1 = await ContactService.FindAsync(instance.ManagerId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Department = await DepartmentService.FindAsync(instance.DepartmentId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(instance);
        }

        // GET: Employee/Create
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.Id = new SelectList(await ContactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ManagerId = new SelectList(await ContactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(await DepartmentService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,ShrtDesc,Designation,DepartmentId,ManagerId,Grade,DOJ,DOL,PFFlag,ESIFlag,FoodFlag,FoodRate,HourlyRate,Employed,HasLeft,Basic,HRA,ExecA,ConvA,TeaA,WashA,CEA,OthA,PFFix,CarLInst,IncomeTAXInst,ProfTAXInst,EduTAXInst,ServiceTAXInst,OtherTAXInst,LifeInsuInst,HealthInsuInst,LoanWInst,LoanWOInst,PFNo,ESINo,Bank,BankAcNo,LifeInsuNo,HealthInsuNo,GratuityNo,PAN,EmpVend,DISPNo,LastPerformanceReviewDate,NextPerformanceReviewDate,LastCompensationReviewDate,NextCompensationReviewDate,Remarks,Authorized,Active")] Employee instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                instance.Id = instance.Id; // This is contactId. Special case for employee
                instance.OrgId = applicationUser.OrgId.ToSysGuid();
                instance.CreatedBy = applicationUser.Id;
                instance.CreatedDate = DateTime.UtcNow;
                instance.ModifiedBy = applicationUser.Id;
                instance.ModifiedDate = DateTime.UtcNow;
                instance.Active = true;
                
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

        // GET: Employee/Edit/5
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Employee instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.ManagerId = new SelectList(await ContactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ManagerId);
            ViewBag.DepartmentId = new SelectList(await DepartmentService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.DepartmentId);
           
            return View(instance);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ShrtDesc,Designation,DepartmentId,ManagerId,Grade,DOJ,DOL,PFFlag,ESIFlag,FoodFlag,FoodRate,HourlyRate,Employed,HasLeft,Basic,HRA,ExecA,ConvA,TeaA,WashA,CEA,OthA,PFFix,CarLInst,IncomeTAXInst,ProfTAXInst,EduTAXInst,ServiceTAXInst,OtherTAXInst,LifeInsuInst,HealthInsuInst,LoanWInst,LoanWOInst,PFNo,ESINo,Bank,BankAcNo,LifeInsuNo,HealthInsuNo,GratuityNo,PAN,EmpVend,DISPNo,LastPerformanceReviewDate,NextPerformanceReviewDate,LastCompensationReviewDate,NextCompensationReviewDate,Remarks,Authorized,Active")] Employee instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Employee model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  HRAdminAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("HRAdminAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remarks = instance.Authorized ? "[Authorized] " + instance.Remarks : "[UnAuthorized] " + instance.Remarks;
                    }
                }
                //If user is not in role of HRAdminAuthorize and record is Authorized, stop editng and show appropriate message.
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
                model.Remarks = instance.Remarks;
                model.ShrtDesc = instance.ShrtDesc;
                model.ManagerId = instance.ManagerId.ToGuid();
                model.DepartmentId = instance.DepartmentId.ToGuid();
                model.Designation = instance.Designation;
                model.Grade = instance.Grade;model.DOJ = instance.DOJ;model.DOL = instance.DOL;model.PFFlag = instance.PFFlag;
                model.ESIFlag = instance.ESIFlag;
                model.FoodFlag = instance.FoodFlag;
                model.FoodRate = instance.FoodRate;
                model.HourlyRate = instance.HourlyRate;
                model.Employed = instance.Employed;
                model.HasLeft = instance.HasLeft;
                model.Basic = instance.Basic;
                model.HRA = instance.HRA;
                model.ExecA = instance.ExecA;
                model.ConvA = instance.ConvA;
                model.TeaA = instance.TeaA;
                model.WashA = instance.WashA;
                model.CEA = instance.CEA;
                model.OthA = instance.OthA;
                model.PFFix = instance.PFFix;
                model.CarLInst = instance.CarLInst;
                model.IncomeTAXInst = instance.IncomeTAXInst;
                model.ProfTAXInst = instance.ProfTAXInst;
                model.EduTAXInst = instance.EduTAXInst;
                model.ServiceTAXInst = instance.ServiceTAXInst;
                model.OtherTAXInst = instance.OtherTAXInst;
                model.LifeInsuInst = instance.LifeInsuInst;
                model.HealthInsuInst = instance.HealthInsuInst;
                model.LoanWInst = instance.LoanWInst;
                model.LoanWOInst = instance.LoanWOInst;
                model.PFNo = instance.PFNo;
                model.ESINo = instance.ESINo;
                model.Bank = instance.Bank;
                model.BankAcNo = instance.BankAcNo;
                model.LifeInsuNo = instance.LifeInsuNo;
                model.HealthInsuNo = instance.HealthInsuNo;
                model.GratuityNo = instance.GratuityNo;
                model.PAN = instance.PAN;
                model.EmpVend = instance.EmpVend;
                model.DISPNo = instance.DISPNo;
                model.LastPerformanceReviewDate = instance.LastPerformanceReviewDate;
                model.NextPerformanceReviewDate = instance.NextPerformanceReviewDate;
                model.LastCompensationReviewDate = instance.LastCompensationReviewDate;
                model.NextCompensationReviewDate = instance.NextCompensationReviewDate;
                model.Remarks = instance.Remarks;
                model.Authorized = instance.Authorized;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Id", "ShrtDesc", "Designation", "DepartmentId", "ManagerId", "Grade", "DOJ", "DOL", "PFFlag", "ESIFlag", "FoodFlag", "FoodRate", "HourlyRate", "Employed", "HasLeft", "Basic", "HRA", "ExecA", "ConvA", "TeaA", "WashA", "CEA", "OthA", "PFFix", "CarLInst", "IncomeTAXInst", "ProfTAXInst", "EduTAXInst", "ServiceTAXInst", "OtherTAXInst", "LifeInsuInst", "HealthInsuInst", "LoanWInst", "LoanWOInst", "PFNo", "ESINo", "Bank", "BankAcNo", "LifeInsuNo", "HealthInsuNo", "GratuityNo", "PAN", "EmpVend", "DISPNo", "LastPerformanceReviewDate", "NextPerformanceReviewDate", "LastCompensationReviewDate", "NextCompensationReviewDate", "Remarks", "Authorized", "Active" };
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

        // GET: Employee/Delete/5
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Employee model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Employee model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: EmployeeId=" + id + " Name=" + model.Name);

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

        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()))
                    .OrderBy(x => x.Name)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
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
