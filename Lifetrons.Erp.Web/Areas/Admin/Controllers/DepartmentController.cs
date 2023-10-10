using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using PagedList;
using Repository;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Admin.Controllers
{
    [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit, HRAdminView")]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public DepartmentController(IDepartmentService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: Department
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: Department/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Department instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            
            ViewBag.DepartmentId = instance.Id;
            ViewBag.DepartmentName = instance.Name;

            return View(instance);
        }

        // GET: Department/Create
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,Authorized")] Department instance)
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
                           : instance.Code + Helper.SysSeparator + DateTime.UtcNow;
              
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
        

        // GET: Department/Edit/5
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Department instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "HRAdminAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("HRAdminAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            return View(instance);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Authorized")] Department instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Department model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Authorized = instance.Authorized;
                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                model.ShrtDesc = instance.ShrtDesc;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

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


        // GET: Department/Delete/5
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Department model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Department model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    if (model.OrgId == applicationUser.OrgId)
                    {
                        _service.Delete(id.ToString());
                        await _unitOfWork.SaveChangesAsync();
                    }
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
