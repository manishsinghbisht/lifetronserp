using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.SysAdmin.Controllers
{
    [EsAuthorize]
    public class OrganizationController : BaseController
    {
        [Dependency]
        public AccountController AccountController { get; set; }

        private readonly IOrganizationService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public OrganizationController(IOrganizationService service, IUnitOfWorkAsync unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        // GET: /Organization
        [EsAuthorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 25);
            // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        [Authorize]
        public ActionResult ManageOrganization()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = new AccountController().GetUserById(User.Identity.GetUserId());
                //Return not found if user already has organization
                if (user.OrgId.HasValue)
                {
                    return HttpNotFound();
                }
                return View();
            }
            return HttpNotFound();
        }

        // GET: /Organization/Details/5
        [Authorize]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Organization instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }
            return View(instance);
        }

        // GET: /Organization/Details/5
        [Authorize]
        public async Task<ActionResult> ShowOrganization()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Organization instance = await _service.FindAsync(applicationUser.OrgId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }
            return View("Details", instance);
        }

        // GET: /Organization/Create
        
        [EsAuthorize]
        public ActionResult Create()
        {
            var user = new AccountController().GetUserById(User.Identity.GetUserId());
            if (user.OrgId.HasValue)
            {
                return HttpNotFound();
            }
            Organization instance = new Organization()
            {
                Email1 = user.AuthenticatedEmail, 
                Email2 = user.AuthenticatedEmail, 
                Phone1 = user.Mobile,
                Phone2 = user.Mobile,
                ApproverEMail = user.AuthenticatedEmail, 
                ApproverPhone = user.Mobile,
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "1000001"
            };
            return View(instance);
        }

        // POST: /Organization/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShortName,Slogan,TIN,RegistrationNumber,Phone1,Phone2,Email1,Email2,AddressLine1,AddressLine2,City,State,Country,PostalCode,ApproverEMail,ApproverPhone,Website")] Organization instance)
        {
            if (!ModelState.IsValid) return View(instance);
            if (User.Identity.IsAuthenticated)
            {
                var user = new AccountController().GetUserById(User.Identity.GetUserId());
                if (user.OrgId == null)
                {
                    instance.Id = Guid.NewGuid();
                    instance.CreatedBy = user.Id;
                    instance.CreatedDate = DateTime.UtcNow;
                    instance.ModifiedBy = user.Id;
                    instance.ModifiedDate = DateTime.UtcNow;
                    instance.Authorized = true;
                    instance.Active = true;

                    instance.Code = string.IsNullOrEmpty(instance.Code) ? instance.Name + Helper.SysSeparator + DateTime.UtcNow : instance.Code + Helper.SysSeparator + DateTime.UtcNow;
                    instance.ShortName = string.IsNullOrEmpty(instance.ShortName)
                        ? "Short Name " + DateTime.UtcNow
                        : instance.ShortName;
                    instance.Slogan = string.IsNullOrEmpty(instance.Slogan)
                        ? "Slogan " + DateTime.UtcNow
                        : instance.Slogan;
                    instance.TIN = string.IsNullOrEmpty(instance.TIN) ? "TIN " + DateTime.UtcNow : instance.TIN;
                    instance.RegistrationNumber = string.IsNullOrEmpty(instance.RegistrationNumber)
                        ? "Registration Number " + DateTime.UtcNow
                        : instance.RegistrationNumber;
                    instance.Website = string.IsNullOrEmpty(instance.Website) ? "Website" + DateTime.UtcNow : instance.Website;

                    try
                    {
                        _service.Create(instance);
                        await _unitOfWork.SaveChangesAsync();

                        //Update user table with organization
                        await new AccountController().UpdateUserOrganization(user.Id, instance.Id.ToString());

                       //Assign roles except authorize and edit. These are already assigned when organization is updated for user
                        AccountController.AddUserToRole(user.Id, "OrganizationLevel");
                        AccountController.AddUserToRole(user.Id, "DepartmentLevel");
                        AccountController.AddUserToRole(user.Id, "TeamLevel");
                        
                        AccountController.AddUserToRole(user.Id, "AllCases");
                        AccountController.AddUserToRole(user.Id, "AllOrders");

                        AccountController.AddUserToRole(user.Id, "HRAdminAuthorize");
                        AccountController.AddUserToRole(user.Id, "HRAdminEdit");
                        AccountController.AddUserToRole(user.Id, "HRAdminView");

                        AccountController.AddUserToRole(user.Id, "LogisticsAuthorize");
                        AccountController.AddUserToRole(user.Id, "LogisticsEdit");
                        AccountController.AddUserToRole(user.Id, "LogisticsView");

                        AccountController.AddUserToRole(user.Id, "PeopleAuthorize");
                        AccountController.AddUserToRole(user.Id, "PeopleEdit");
                        AccountController.AddUserToRole(user.Id, "PeopleView");

                        AccountController.AddUserToRole(user.Id, "ProcurementAuthorize");
                        AccountController.AddUserToRole(user.Id, "ProcurementEdit");
                        AccountController.AddUserToRole(user.Id, "ProcurementView");

                        AccountController.AddUserToRole(user.Id, "PriceBookManager");
                        AccountController.AddUserToRole(user.Id, "ProductsAuthorize");
                        AccountController.AddUserToRole(user.Id, "ProductsEdit");
                        AccountController.AddUserToRole(user.Id, "ProductsView");

                        AccountController.AddUserToRole(user.Id, "SalesAuthorize");
                        AccountController.AddUserToRole(user.Id, "SalesEdit");
                        AccountController.AddUserToRole(user.Id, "SalesView");

                        AccountController.AddUserToRole(user.Id, "ServicesAuthorize");
                        AccountController.AddUserToRole(user.Id, "ServicesEdit");
                        AccountController.AddUserToRole(user.Id, "ServicesView");

                        AccountController.AddUserToRole(user.Id, "StockAuthorize");
                        AccountController.AddUserToRole(user.Id, "StockEdit");
                        AccountController.AddUserToRole(user.Id, "StockView");

                        AccountController.AddUserToRole(user.Id, "SysAdminAuthorize");
                        AccountController.AddUserToRole(user.Id, "SysAdminEdit");
                        AccountController.AddUserToRole(user.Id, "SysAdminView");

                        AccountController.AddUserToRole(user.Id, "WorksAuthorize");
                        AccountController.AddUserToRole(user.Id, "WorksEdit");
                        AccountController.AddUserToRole(user.Id, "WorksView");
                        AccountController.AddUserToRole(user.Id, "WorksPlanner");

                        //AccountController.AddUserToRole(user.Id, "canEdit");

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
                    catch (Exception ex)
                    {
                        HandleException(ex);
                        throw;
                    }

                    try
                    {
                        //Set up organization
                        var db = new Entities();
                        db.spSetupOrg(instance.Id, user.Id);
                    }
                    catch (Exception ex)
                    {
                            
                      
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }


            return RedirectToAction("Details", new { id = instance.Id });
        }

        // GET: /Organization/Edit/5
        [EsAuthorize(Roles = "SuperAdmin, OrganizationLevel")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Organization instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }
            return View(instance);
        }

        // POST: /Organization/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SuperAdmin, OrganizationLevel")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShortName,Slogan,TIN,RegistrationNumber,Phone1,Phone2,Email1,Email2,AddressLine1,AddressLine2,City,State,Country,PostalCode,ApproverEMail,ApproverPhone,Website")] Organization instance)
        {
            if (ModelState.IsValid)
            {
                var user = new AccountController().GetUserById(User.Identity.GetUserId());
                Organization model = await _service.FindAsync(instance.Id.ToString(), user.Id, user.OrgId.ToString());

                model.ObjectState = ObjectState.Modified;

                model.ModifiedBy = user.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Authorized = instance.Authorized;
                
                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code) ? instance.Name + Helper.SysSeparator + DateTime.UtcNow : instance.Code;
                model.ShortName = string.IsNullOrEmpty(instance.ShortName) ? "Short Name " + DateTime.UtcNow : instance.ShortName;
                model.Slogan = string.IsNullOrEmpty(instance.Slogan) ? "Slogan " + DateTime.UtcNow : instance.Slogan;
                model.TIN = string.IsNullOrEmpty(instance.TIN) ? "TIN " + DateTime.UtcNow : instance.TIN;
                model.RegistrationNumber = string.IsNullOrEmpty(instance.RegistrationNumber) ? "Registration Number " + DateTime.UtcNow : instance.RegistrationNumber;
                model.Website = string.IsNullOrEmpty(instance.Website) ? "Website" + DateTime.UtcNow : instance.Website;

                model.Phone1 = instance.Phone1;
                model.Phone2 = instance.Phone2;
                model.Email1 = instance.Email1;
                model.Email2 = instance.Email2;
                model.AddressLine1 = instance.AddressLine1;
                model.AddressLine2 = instance.AddressLine2;
                model.City = instance.City;
                model.State = instance.State;
                model.Country = instance.Country;
                model.PostalCode = instance.PostalCode;
                model.ApproverEMail = instance.ApproverEMail;
                model.ApproverPhone = instance.ApproverPhone;

                try
                {
                    _service.Update(model);
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
                catch (Exception ex)
                {
                    HandleException(ex);
                    throw;
                }

                return RedirectToAction("Details", new { id = instance.Id });
            }
            return View(instance);
        }

        //// GET: /Organization/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Organization model = await _service.FindAsync(id);
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(model);
        //}

        // POST: /Organization/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    _service.Delete(id);
        //    await _unitOfWork.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        [EsAuthorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()))
                    .OrderBy(x => x.Name)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
        }

        #region Handle Exception
        private void HandleException(Exception exception)
        {
            string innerExceptionMessage = exception.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = innerExceptionMessage;
            if (innerExceptionMessage.Contains("UQ"))
            {
                TempData["CustomErrorMessage"] = "You may have entered a alredy existing value for a field which should be unique.Ex: TIN, Website etc. See the detail message.";
                TempData["CustomErrorDetail"] =  innerExceptionMessage;
            }
        }

        private void HandleDbUpdateException(DbUpdateException exception)
        {
            string innerExceptionMessage = exception.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = innerExceptionMessage;

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
