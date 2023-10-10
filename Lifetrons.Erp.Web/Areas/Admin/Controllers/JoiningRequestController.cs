using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Microsoft.Practices.Unity;

namespace Lifetrons.Erp.Admin.Controllers
{
    [EsAuthorize]
    public class JoiningRequestController : BaseController
    {
        [Dependency]
        public AccountController AccountController { get; set; }

        private readonly IJoiningRequestService _service;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public JoiningRequestController(IJoiningRequestService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: /JoiningRequest/
        [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit")]
        public async Task<ActionResult> Index()
        {

            var currentUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (currentUser.OrgId != null) //Only approver is allwed to view requests
            {
                return View(await _service.GetAsync(currentUser.Id, currentUser.OrgId.ToString()));
            }
            else
            {
                return HttpNotFound();
            }
        }

        // GET: /JoiningRequest/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instance = await _service.FindAsyncEnumerable(id);
            if (instance == null)
            {
                return HttpNotFound();
            }
            return View(instance);
        }

        // GET: /JoiningRequest/Create
        public ActionResult Create()
        {

            //ViewBag.RequestBy = new SelectList(await _aspNetUserService.GetAsync(), "Id", "UserName");
            //ViewBag.SubmittedTo = new SelectList(await _aspNetUserService.GetAsync(), "Id", "UserName");
            return View();
        }

        // POST: /JoiningRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RequestComments,SubmittedTo")] JoiningRequest instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (currentUser.OrgId == null)
                {
                    instance.Id = Guid.NewGuid();
                    instance.RequestType = "JOINING";
                    instance.RequestBy = currentUser.Id;
                    instance.RequestDate = DateTime.UtcNow;
                    instance.Status = "PENDING";

                    var submitttedToUser = new AccountController().GetUserByName(instance.SubmittedTo);
                    if (submitttedToUser != null)
                    {
                        instance.SubmittedTo = submitttedToUser.Id;
                    }
                    else
                    {
                        TempData["CustomErrorMessage"] = "Invalid username";
                        TempData["CustomErrorDetail"] = "No user exist with the username given.";
                        throw new ApplicationException("Invalid username");
                    }

                    //Auto Approve if user request to Join Demo organization
                    if(submitttedToUser.OrgId.ToString().ToUpper() == Lifetrons.Erp.Data.Helper.SupportUserOrgId.ToUpper() && Lifetrons.Erp.Data.Helper.AppHomeURL.ToUpper().Contains("S1.EASYSALES.IN"))
                    {
                        instance.Status = "APPROVED";
                        instance.ApprovedBy = Lifetrons.Erp.Data.Helper.SupportUserId.ToString();
                        instance.ApprovedDate = DateTime.UtcNow;
                        instance.ResponseComments = "Joining auto approved by system";
                        await new AccountController().UpdateUserOrganization(instance.RequestBy,
                            Lifetrons.Erp.Data.Helper.SupportUserOrgId.ToString());

                        //Assign roles except authorize and edit. These are already assigned when organization is updated for user
                        AccountController.AddUserToRole(currentUser.Id, "OrganizationLevel");
                        AccountController.AddUserToRole(currentUser.Id, "DepartmentLevel");
                        AccountController.AddUserToRole(currentUser.Id, "TeamLevel");

                        AccountController.AddUserToRole(currentUser.Id, "AllCases");
                        AccountController.AddUserToRole(currentUser.Id, "AllOrders");

                        AccountController.AddUserToRole(currentUser.Id, "HRAdminAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "HRAdminEdit");
                        AccountController.AddUserToRole(currentUser.Id, "HRAdminView");

                        AccountController.AddUserToRole(currentUser.Id, "LogisticsAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "LogisticsEdit");
                        AccountController.AddUserToRole(currentUser.Id, "LogisticsView");

                        AccountController.AddUserToRole(currentUser.Id, "PeopleAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "PeopleEdit");
                        AccountController.AddUserToRole(currentUser.Id, "PeopleView");

                        AccountController.AddUserToRole(currentUser.Id, "ProcurementAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "ProcurementEdit");
                        AccountController.AddUserToRole(currentUser.Id, "ProcurementView");

                        AccountController.AddUserToRole(currentUser.Id, "PriceBookManager");
                        AccountController.AddUserToRole(currentUser.Id, "ProductsAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "ProductsEdit");
                        AccountController.AddUserToRole(currentUser.Id, "ProductsView");

                        AccountController.AddUserToRole(currentUser.Id, "SalesAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "SalesEdit");
                        AccountController.AddUserToRole(currentUser.Id, "SalesView");

                        AccountController.AddUserToRole(currentUser.Id, "ServicesAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "ServicesEdit");
                        AccountController.AddUserToRole(currentUser.Id, "ServicesView");

                        AccountController.AddUserToRole(currentUser.Id, "StockAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "StockEdit");
                        AccountController.AddUserToRole(currentUser.Id, "StockView");

                        AccountController.AddUserToRole(currentUser.Id, "SysAdminAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "SysAdminEdit");
                        AccountController.AddUserToRole(currentUser.Id, "SysAdminView");

                        AccountController.AddUserToRole(currentUser.Id, "WorksAuthorize");
                        AccountController.AddUserToRole(currentUser.Id, "WorksEdit");
                        AccountController.AddUserToRole(currentUser.Id, "WorksView");
                        AccountController.AddUserToRole(currentUser.Id, "WorksPlanner");

                        //AccountController.AddUserToRole(user.Id, "canEdit");

                        _service.Create(instance);
                        await _unitOfWork.SaveChangesAsync();

                        //Log Off or Sign Out
                        HttpContext.GetOwinContext().Authentication.SignOut();
                        return RedirectToAction("Index", "Home", new { area = ""});
                    }

                    _service.Create(instance);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return RedirectToAction("Details", new { id = instance.Id });
        }

        // GET: /JoiningRequest/Edit/5
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        public async Task<ActionResult> Edit(string id)
        {
            var currentUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (currentUser.OrgId != null) //Only approver is allowed
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                JoiningRequest instance = await _service.FindAsync(id);

                if (instance == null || instance.Status != "PENDING")
                {
                    return HttpNotFound("Access not allowed.");
                }

                var requestUser = new AccountController().GetUserById(instance.RequestBy);
                instance.AspNetUser = new AspNetUser();
                instance.AspNetUser.Id = requestUser.Id;
                instance.AspNetUser.UserName = requestUser.UserName;
                instance.AspNetUser.OrgId = requestUser.OrgId;

                var submittedUser = new AccountController().GetUserById(instance.SubmittedTo);
                instance.AspNetUser1 = new AspNetUser();
                instance.AspNetUser1.Id = submittedUser.Id;
                instance.AspNetUser1.UserName = submittedUser.UserName;
                instance.AspNetUser1.OrgId = submittedUser.OrgId;


                return View(instance);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: /JoiningRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "HRAdminAuthorize")]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Status, ResponseComments")] JoiningRequest instance)
        {

            JoiningRequest model = await _service.FindAsync(instance.Id.ToString());

            var currentUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (currentUser.OrgId != null) //Only approver is allowed
            {
                model.ObjectState = ObjectState.Modified;
                model.ApprovedBy = currentUser.Id;
                model.ApprovedDate = DateTime.UtcNow;
                model.ResponseComments = instance.ResponseComments;
                model.Status = instance.Status;

                ModelState.Clear();
                if (TryValidateModel(model))
                {
                    _service.Update(model);
                    await _unitOfWork.SaveChangesAsync();

                    if (model.Status.ToUpper() == "APPROVED")
                    {
                        model.Status = instance.Status.ToUpper();
                        await new AccountController().UpdateUserOrganization(model.RequestBy,
                            currentUser.OrgId.ToString());
                    }
                    return RedirectToAction("Details", new { id = model.Id });
                }
                else
                {
                    var requestUser = new AccountController().GetUserById(model.RequestBy);
                    model.AspNetUser = new AspNetUser();
                    model.AspNetUser.Id = requestUser.Id;
                    model.AspNetUser.UserName = requestUser.UserName;
                    model.AspNetUser.OrgId = requestUser.OrgId;

                    var submittedUser = new AccountController().GetUserById(model.SubmittedTo);
                    model.AspNetUser1 = new AspNetUser();
                    model.AspNetUser1.Id = submittedUser.Id;
                    model.AspNetUser1.UserName = submittedUser.UserName;
                    model.AspNetUser1.OrgId = submittedUser.OrgId;
                }
            }
            return View(model);
        }

        //// GET: /JoiningRequest/Delete/5
        //public async Task<ActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    JoiningRequest model = await _service.FindAsync(id.ToString());
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(model);
        //}

        //// POST: /JoiningRequest/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(Guid id)
        //{
        //    _service.Delete(id.ToString());
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
    }
}
