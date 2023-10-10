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
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class CampaignMemberController : BaseController
    {
        private readonly ICampaignMemberService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly ICampaignMemberStatusService _campaignMemberStatusService;
        private readonly IContactService _contactService;
        private readonly ILeadService _leadService;
        private readonly ICampaignService _campaignService;

        public CampaignMemberController(ICampaignMemberService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService,
            ICampaignMemberStatusService campaignMemberStatusService, IContactService contactService, ILeadService leadService, ICampaignService campaignService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _campaignMemberStatusService = campaignMemberStatusService;
            _contactService = contactService;
            _leadService = leadService;
            _campaignService = campaignService;
        }


        // GET: CampaignMember
        public ActionResult Index(string campaignId)
        {
            ViewBag.CampaignId = campaignId;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(campaignId, applicationUser.Id, applicationUser.OrgId.ToString());

            return PartialView(model);
        }

        // GET: CampaignMember/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //ProductType instance = await _service.FindAsync(id);
            CampaignMember instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified

            instance.Campaign = await _campaignService.FindAsync(instance.CampaignId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Lead = await _leadService.FindAsync(instance.LeadId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact = await _contactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.CampaignMemberStatu = await _campaignMemberStatusService.FindAsync(instance.CampaignMemberStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(instance);
        }

        // GET: CampaignMember/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create(string campaignId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.CampaignMemberStatusId = new SelectList(await _campaignMemberStatusService.SelectAsyncLineItems(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.LeadId = new SelectList(await _leadService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            ViewBag.CampaignId = campaignId;
            ViewBag.CampaignName = (await _campaignService.FindAsync(campaignId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View();
        }

        // POST: CampaignMember/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,CampaignId,LeadId,ContactId,CampaignMemberStatusId,ShrtDesc,Authorized")] CampaignMember instance)
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

                instance.CampaignId = instance.CampaignId.ToString().ToSysGuid();
                instance.LeadId = instance.LeadId.ToGuid();
                instance.ContactId = instance.ContactId.ToGuid();
                instance.CampaignMemberStatusId = instance.CampaignMemberStatusId.ToGuid();

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

        // GET: CampaignMember/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            CampaignMember instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
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

            ViewBag.CampaignId = instance.CampaignId;
            ViewBag.CampaignName = (await _campaignService.FindAsync(instance.CampaignId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.LeadId = new SelectList(await _leadService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LeadId);
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ContactId);
            ViewBag.CampaignMemberStatusId = new SelectList(await _campaignMemberStatusService.SelectAsyncLineItems(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CampaignMemberStatusId);

            return View(instance);
        }

        // POST: CampaignMember/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CampaignId,LeadId,ContactId,CampaignMemberStatusId,ShrtDesc,Authorized")] CampaignMember instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                CampaignMember model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  SalesAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("SalesAuthorize"))
                {
                    model.Authorized = instance.Authorized;
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

                model.CampaignId = instance.CampaignId.ToString().ToSysGuid();
                model.ContactId = instance.ContactId.ToGuid();
                model.LeadId = instance.LeadId.ToGuid();
                model.CampaignMemberStatusId = instance.CampaignMemberStatusId;
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

        // GET: CampaignMember/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            CampaignMember model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: CampaignMember/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    CampaignMember model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                if (innerExceptionMessage.Contains("LeadId_ContactId"))
                {
                    TempData["CustomErrorMessage"] = "Selected Lead or Contact already added to this campaign.";
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
