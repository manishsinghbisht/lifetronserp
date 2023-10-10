using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using PagedList;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Admin.Controllers
{
    [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit, HRAdminView")]
    public class HierarchyController : BaseController
    {
        private readonly IHierarchyService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IDepartmentService _departmentService;
        private readonly ITeamService _teamService;

        public HierarchyController(IHierarchyService service, ITeamService teamService, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService, IDepartmentService departmentService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _departmentService = departmentService;
            _teamService = teamService;
        }


        // GET: Hierarchy
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.DepartmentId).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: Hierarchy/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Hierarchy instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ReportsTo); 
            instance.Department = _departmentService.Find(instance.DepartmentId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Team = await _teamService.FindAsync(instance.TeamId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            
            //AspNetUser is UserId and Primary Key. This must be populated in the end to save from an EF6 issue of PK
            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.UserId); 

            ViewBag.UserId = instance.UserId;
            
            return View(instance);
        }

        // GET: Hierarchy/Create
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.UserId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.DepartmentId = new SelectList(await _departmentService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            //ViewBag.TeamId = new SelectList(await _teamService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.TeamId = new SelectList("");
            ViewBag.ReportsTo = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            return View();
        }

        // POST: Hierarchy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserId,DepartmentId,TeamId,ReportsTo,Designation")] Hierarchy instance)
        {
            if (User.Identity.IsAuthenticated)
            {
               var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                if (instance.DepartmentId == Guid.Empty || instance.TeamId == Guid.Empty || instance.ReportsTo == "")
                {
                    System.ArgumentNullException exception = new ArgumentNullException("Department, Team, ReportsTo are required fields.");
                    TempData["CustomErrorMessage"] = "Department, Team, ReportsTo are required fields.";
                    TempData["CustomErrorDetail"] = exception.Message.GetType();
                    throw exception;
                }

                try
                {
                    instance.OrgId = applicationUser.OrgId.ToSysGuid();
                    instance.UserId = instance.UserId.ToSysGuid().ToString();
                    instance.DepartmentId = instance.DepartmentId.ToGuid();
                    instance.TeamId = instance.TeamId.ToGuid();
                    instance.ReportsTo = instance.ReportsTo.ToSysGuid().ToString();

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

            return RedirectToAction("Details", new { id = instance.UserId });
        }


        // GET: Hierarchy/Edit/5
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Hierarchy instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            TempData["username"] = ControllerHelper.GetAspNetUser(instance.UserId).UserName; 
            //ViewBag.UserId = new SelectList(await _aspNetUserService.GetAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.UserId);
            ViewBag.DepartmentId = new SelectList(await _departmentService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.DepartmentId);
            ViewBag.TeamId = new SelectList(await _teamService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString(), instance.DepartmentId.ToString()), "Id", "Name", instance.TeamId);
            ViewBag.ReportsTo = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.ReportsTo);
            
            return View(instance);
        }

        // POST: Hierarchy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "UserId,DepartmentId,TeamId,ReportsTo,Designation")] Hierarchy instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Hierarchy model = await _service.FindAsync(instance.UserId, applicationUser.Id, applicationUser.OrgId.ToString());

                model.ObjectState = ObjectState.Modified;
                model.OrgId = applicationUser.OrgId.ToSysGuid();
                model.DepartmentId = instance.DepartmentId.ToGuid();
                model.TeamId = instance.TeamId.ToGuid();
                model.ReportsTo = instance.ReportsTo.ToSysGuid().ToString();
                model.Designation = instance.Designation;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        return RedirectToAction("Details", new { id = model.UserId });
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

        // GET: Hierarchy/Delete/5
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Hierarchy model = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Hierarchy/Delete/5
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Hierarchy model = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
                    if (model.OrgId == applicationUser.OrgId)
                    {
                        _service.Delete(id);
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

       /// <summary>
       /// This method is called to update dropdown through ajax request
       /// </summary>
       /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
       /// <returns>Java script serialized string array to be used with JSON parsing</returns>
       public async virtual Task<JsonResult> ProcessJsonResponseForTeamDdl(string stringifiedParam)
       {
           var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

           //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
           var param = stringifiedParam;

           var response = new JsonResult();
           response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

           var select = new SelectList(await _teamService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString(), stringifiedParam), "Id", "Name");

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

        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                records.Where(x => x.AspNetUser.UserName.ToLower().Contains(searchParam))
                    .OrderBy(x => x.AspNetUser.Name)
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
                    TempData["CustomErrorMessage"] = "Heirarchy already defined for the username. Key record already exists.";
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
