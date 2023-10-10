using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
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
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class TargetController : BaseController
    {
        private readonly ITargetService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        [Dependency]
        public IOrganizationService OrganizationService { get; set; }

        [Dependency]
        public IDepartmentService DepartmentService { get; set; }

        [Dependency]
        public ITeamService TeamService { get; set; }

        public TargetController(ITargetService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }


        // GET: /Target/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.TargetDate).ThenByDescending(x => x.ObjectName).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            //This should be done after sorting
            int i = 0;
            enumerablePage.ForEach(p =>
            {
                TempData["ObjectIdName" + i] = GetObjectIdName(p.ObjectName, p.ObjectId.ToString());
                i++;
            });

            return View(enumerablePage);
        }

        // GET: Target/Details/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Lifetrons.Erp.Data.Target instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId); //Owner
            
            TempData["ObjectIdName"] = GetObjectIdName(instance.ObjectName, instance.ObjectId.ToString());

            return View(instance);
        }

       // GET: Target/Create
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);

            return View();
        }

        // POST: Target/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel")]
        public async Task<ActionResult> Create([Bind(Include = "ObjectName,ObjectId,TargetDate,TargetFigure,ShrtDesc,Desc,ClosingComments,OwnerId,SharedWith,Authorized")] Target instance)
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
                var lastDayOfMonth = DateTime.DaysInMonth(instance.TargetDate.Year, instance.TargetDate.Month);
                instance.TargetDate = new  DateTime(instance.TargetDate.Year, instance.TargetDate.Month, lastDayOfMonth); // No need of UTC conversion. Target date is already in UTC
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

        // GET: Target/Edit/5
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lifetrons.Erp.Data.Target instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "SalesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("SalesAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnerId);
            TempData["ObjectIdName"] = GetObjectIdName(instance.ObjectName, instance.ObjectId.ToString());

            return View(instance);
        }

        // POST: Target/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ObjectName,ObjectId,TargetDate,TargetFigure,ShrtDesc,Desc,ClosingComments,OwnerId,SharedWith,Authorized,TimeStamp")] Target instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Lifetrons.Erp.Data.Target model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  SalesAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("SalesAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
                    }
                }
                //If user is not in role of SalesAuthorize and record is Authorized, stop editng and show appropriate message.
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
                model.Desc = instance.Desc;
                model.OwnerId = instance.OwnerId;
                model.ObjectName = instance.ObjectName;
                model.ObjectId = instance.ObjectId;
                var lastDayOfMonth = DateTime.DaysInMonth(instance.TargetDate.Year, instance.TargetDate.Month);
                model.TargetDate = new DateTime(instance.TargetDate.Year, instance.TargetDate.Month, lastDayOfMonth);
                model.TargetFigure = instance.TargetFigure;
                model.ClosingComments = instance.ClosingComments;

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


        // GET: Target/Delete/5
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lifetrons.Erp.Data.Target model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /Default1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Lifetrons.Erp.Data.Target model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    //if (model.OrgId == applicationUser.OrgId)
                    //{
                    //    _service.Delete(id.ToString());
                    //    await _unitOfWork.SaveChangesAsync();
                    //}

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

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
        /// Used in Edit and Detail View
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public string GetObjectIdName(string objectName, string objectId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            string relatedToIdName = string.Empty;
            switch (objectName)
            {
                case "User":
                    relatedToIdName = _aspNetUserService.Find(objectId).Name;
                    break;
                case "Team":
                    relatedToIdName = TeamService.Find(objectId, applicationUser.Id, applicationUser.OrgId.ToString()).Name;
                    break;
                case "Department":
                    relatedToIdName = DepartmentService.Find(objectId, applicationUser.Id, applicationUser.OrgId.ToString()).Name;
                    break;
                case "Organization":
                    relatedToIdName = OrganizationService.Find(objectId, applicationUser.Id, applicationUser.OrgId.ToString()).Name;
                    break;

            }

            return relatedToIdName;
        }

        /// <summary>
        /// This method is called to update dropdown through ajax request
        /// </summary>
        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
        public async virtual Task<JsonResult> ProcessJsonResponseForObjectIdDdl(string stringifiedParam)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
            var param = stringifiedParam;

            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var select = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name");
            switch (param)
            {
                case "User":
                    select = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Team":
                    select = new SelectList(await TeamService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Department":
                    select = new SelectList(await DepartmentService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Organization":
                    select = new SelectList(await OrganizationService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
            }

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
