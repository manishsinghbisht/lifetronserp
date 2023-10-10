using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;

namespace Lifetrons.Erp.SysAdmin.Controllers
{

    [EsAuthorize(Roles = "SuperAdmin, Support, SysAdminAuthorize, SysAdminEdit, SysAdminView")]
    public class RolesController : BaseController
    {
        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public AccountController accountController { get; set; }

        // GET: Roles
        [EsAuthorize(Roles = "OrganizationLevel")]
        [HttpGet]
        public async Task<ActionResult> ManageUserRoles(string userId)
        {
            var user = new AccountController().GetUserById(userId);

            TempData["userId"] = user.Id;
            TempData["userName"] = user.UserName;

            var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            var userRoles = userManager.GetRoles(user.Id);

            //TempData["canEdit"] = false;
            //TempData["canAuthorize"] = false;
            TempData["PriceBookManager"] = false;
            TempData["AllCases"] = false;
            TempData["AllOrders"] = false;
            TempData["TeamLevel"] = false;
            TempData["DepartmentLevel"] = false;
            TempData["OrganizationLevel"] = false;

            TempData["HRAdminAuthorize"] = false;
            TempData["HRAdminEdit"] = false;
            TempData["HRAdminView"] = false;

            TempData["LogisticsAuthorize"] = false;
            TempData["LogisticsEdit"] = false;
            TempData["LogisticsView"] = false;

            TempData["PeopleAuthorize"] = false;
            TempData["PeopleEdit"] = false;
            TempData["PeopleView"] = false;

            TempData["ProcurementAuthorize"] = false;
            TempData["ProcurementEdit"] = false;
            TempData["ProcurementView"] = false;

            TempData["ProductsAuthorize"] = false;
            TempData["ProductsEdit"] = false;
            TempData["ProductsView"] = false;

            TempData["SalesAuthorize"] = false;
            TempData["SalesEdit"] = false;
            TempData["SalesView"] = false;

            TempData["ServicesAuthorize"] = false;
            TempData["ServicesEdit"] = false;
            TempData["ServicesView"] = false;

            TempData["StockAuthorize"] = false;
            TempData["StockEdit"] = false;
            TempData["StockView"] = false;

            TempData["SysAdminAuthorize"] = false;
            TempData["SysAdminEdit"] = false;
            TempData["SysAdminView"] = false;

            TempData["WorksAuthorize"] = false;
            TempData["WorksEdit"] = false;
            TempData["WorksView"] = false;
            TempData["WorksPlanner"] = false;

            TempData["FileAuthorize"] = false;
            TempData["FileUpload"] = false;
            TempData["FileDownload"] = false;
            TempData["FileAdmin"] = false;
            
            //if (userRoles.Contains("canEdit"))
            //{
            //    TempData["canEdit"] = true;
            //}
            //if (userRoles.Contains("canAuthorize"))
            //{
            //    TempData["canAuthorize"] = true;
            //}
            if (userRoles.Contains("PriceBookManager"))
            {
                TempData["PriceBookManager"] = true;
            }
            if (userRoles.Contains("AllCases"))
            {
                TempData["AllCases"] = true;
            }
            if (userRoles.Contains("AllOrders"))
            {
                TempData["AllOrders"] = true;
            }
            if (userRoles.Contains("TeamLevel"))
            {
                TempData["TeamLevel"] = true;
            }
            if (userRoles.Contains("DepartmentLevel"))
            {
                TempData["DepartmentLevel"] = true;
            }
            if (userRoles.Contains("OrganizationLevel"))
            {
                TempData["OrganizationLevel"] = true;
            }
            if (userRoles.Contains("HRAdminAuthorize"))
            {
                TempData["HRAdminAuthorize"] = true;
            }
            if (userRoles.Contains("HRAdminEdit"))
            {
                TempData["HRAdminEdit"] = true;
            }
            if (userRoles.Contains("HRAdminView"))
            {
                TempData["HRAdminView"] = true;
            }
            if (userRoles.Contains("LogisticsAuthorize"))
            {
                TempData["LogisticsAuthorize"] = true;
            }
            if (userRoles.Contains("LogisticsEdit"))
            {
                TempData["LogisticsEdit"] = true;
            }

            if (userRoles.Contains("LogisticsView"))
            {
                TempData["LogisticsView"] = true;
            }
            if (userRoles.Contains("PeopleAuthorize"))
            {
                TempData["PeopleAuthorize"] = true;
            }

            if (userRoles.Contains("PeopleEdit"))
            {
                TempData["PeopleEdit"] = true;
            }
            if (userRoles.Contains("PeopleView"))
            {
                TempData["PeopleView"] = true;
            }
            if (userRoles.Contains("ProcurementAuthorize"))
            {
                TempData["ProcurementAuthorize"] = true;
            }
            if (userRoles.Contains("ProcurementEdit"))
            {
                TempData["ProcurementEdit"] = true;
            }

            if (userRoles.Contains("ProcurementView"))
            {
                TempData["ProcurementView"] = true;
            }
            if (userRoles.Contains("ProductsAuthorize"))
            {
                TempData["ProductsAuthorize"] = true;
            }
            if (userRoles.Contains("ProductsEdit"))
            {
                TempData["ProductsEdit"] = true;
            }
            if (userRoles.Contains("ProductsView"))
            {
                TempData["ProductsView"] = true;
            }

            if (userRoles.Contains("SalesAuthorize"))
            {
                TempData["SalesAuthorize"] = true;
            }

            if (userRoles.Contains("SalesEdit"))
            {
                TempData["SalesEdit"] = true;
            }
            if (userRoles.Contains("SalesView"))
            {
                TempData["SalesView"] = true;
            }
            if (userRoles.Contains("ServicesAuthorize"))
            {
                TempData["ServicesAuthorize"] = true;
            }

            if (userRoles.Contains("ServicesEdit"))
            {
                TempData["ServicesEdit"] = true;
            }
            if (userRoles.Contains("ServicesView"))
            {
                TempData["ServicesView"] = true;
            }
            if (userRoles.Contains("StockAuthorize"))
            {
                TempData["StockAuthorize"] = true;
            }

            if (userRoles.Contains("StockEdit"))
            {
                TempData["StockEdit"] = true;
            }
            if (userRoles.Contains("StockView"))
            {
                TempData["StockView"] = true;
            }
            if (userRoles.Contains("SysAdminAuthorize"))
            {
                TempData["SysAdminAuthorize"] = true;
            }
            if (userRoles.Contains("SysAdminEdit"))
            {
                TempData["SysAdminEdit"] = true;
            }
            if (userRoles.Contains("SysAdminView"))
            {
                TempData["SysAdminView"] = true;
            }

            if (userRoles.Contains("WorksAuthorize"))
            {
                TempData["WorksAuthorize"] = true;
            }
            if (userRoles.Contains("WorksEdit"))
            {
                TempData["WorksEdit"] = true;
            }
            if (userRoles.Contains("WorksView"))
            {
                TempData["WorksView"] = true;
            }
            if (userRoles.Contains("WorksPlanner"))
            {
                TempData["WorksPlanner"] = true;
            }

            if (userRoles.Contains("FileAuthorize"))
            {
                TempData["FileAuthorize"] = true;
            }
            if (userRoles.Contains("FileDownload"))
            {
                TempData["FileDownload"] = true;
            }
            if (userRoles.Contains("FileUpload"))
            {
                TempData["FileUpload"] = true;
            }
            if (userRoles.Contains("FileAdmin"))
            {
                TempData["FileAdmin"] = true;
            }
            return View();
        }

        [EsAuthorize(Roles = "OrganizationLevel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public ActionResult ManageUserRoles(FormCollection formCollection)
        {
            string userId = formCollection["userId"];

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var user = new AccountController().GetUserById(userId);

            if (applicationUser.OrgId != user.OrgId)
            {
                var exception = new System.ApplicationException("Roles: Organization match failed.");
                TempData["CustomErrorMessage"] = "Roles: Organization match failed.";
                throw exception;
            }

            ////Edit
            //if (Convert.ToBoolean(formCollection["canEdit"]))
            //    accountController.AddUserToRole(userId, "canEdit");
            //else
            //    accountController.RemoveUserToRole(userId, "canEdit");

            ////Authorize
            //if (Convert.ToBoolean(formCollection["canAuthorize"]))
            //{
            //    accountController.AddUserToRole(userId, "canAuthorize");
            //}
            //else
            //{
            //    accountController.RemoveUserToRole(userId, "canAuthorize");
            //}

            //PriceBookManager
            if (Convert.ToBoolean(formCollection["PriceBookManager"]))
            {
                accountController.AddUserToRole(userId, "PriceBookManager");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "PriceBookManager");
            }

            //AllCases
            if (Convert.ToBoolean(formCollection["AllCases"]))
            {
                accountController.AddUserToRole(userId, "AllCases");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "AllCases");
            }

            //AllOrders
            if (Convert.ToBoolean(formCollection["AllOrders"]))
            {
                accountController.AddUserToRole(userId, "AllOrders");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "AllOrders");
            }

            //TeamLevel
            if (Convert.ToBoolean(formCollection["TeamLevel"]))
            {
                accountController.AddUserToRole(userId, "TeamLevel");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "TeamLevel");
            }
            //DepartmentLevel
            if (Convert.ToBoolean(formCollection["DepartmentLevel"]))
            {
                accountController.AddUserToRole(userId, "DepartmentLevel");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "DepartmentLevel");
            }
            //OrganizationLevel
            if (Convert.ToBoolean(formCollection["OrganizationLevel"]))
            {
                accountController.AddUserToRole(userId, "OrganizationLevel");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "OrganizationLevel");
            }

            //HRAdminAuthorize
            if (Convert.ToBoolean(formCollection["HRAdminAuthorize"]))
            {
                accountController.AddUserToRole(userId, "HRAdminAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "HRAdminAuthorize");
            }

            //HRAdminEdit
            if (Convert.ToBoolean(formCollection["HRAdminEdit"]))
            {
                accountController.AddUserToRole(userId, "HRAdminEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "HRAdminEdit");
            }

            //HRAdminView
            if (Convert.ToBoolean(formCollection["HRAdminView"]))
            {
                accountController.AddUserToRole(userId, "HRAdminView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "HRAdminView");
            }

            //LogisticsAuthorize
            if (Convert.ToBoolean(formCollection["LogisticsAuthorize"]))
            {
                accountController.AddUserToRole(userId, "LogisticsAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "LogisticsAuthorize");
            }

            //LogisticsEdit
            if (Convert.ToBoolean(formCollection["LogisticsEdit"]))
            {
                accountController.AddUserToRole(userId, "LogisticsEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "LogisticsEdit");
            }

            //LogisticsView
            if (Convert.ToBoolean(formCollection["LogisticsView"]))
            {
                accountController.AddUserToRole(userId, "LogisticsView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "LogisticsView");
            }

            //PeopleAuthorize
            if (Convert.ToBoolean(formCollection["PeopleAuthorize"]))
            {
                accountController.AddUserToRole(userId, "PeopleAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "PeopleAuthorize");
            }

            //PeopleAuthorize
            if (Convert.ToBoolean(formCollection["PeopleEdit"]))
            {
                accountController.AddUserToRole(userId, "PeopleEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "PeopleEdit");
            }

            //PeopleView
            if (Convert.ToBoolean(formCollection["PeopleView"]))
            {
                accountController.AddUserToRole(userId, "PeopleView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "PeopleView");
            }

            //ProcurementAuthorize
            if (Convert.ToBoolean(formCollection["ProcurementAuthorize"]))
            {
                accountController.AddUserToRole(userId, "ProcurementAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ProcurementAuthorize");
            }

            //ProcurementEdit
            if (Convert.ToBoolean(formCollection["ProcurementEdit"]))
            {
                accountController.AddUserToRole(userId, "ProcurementEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ProcurementEdit");
            }

            //ProcurementView
            if (Convert.ToBoolean(formCollection["ProcurementView"]))
            {
                accountController.AddUserToRole(userId, "ProcurementView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ProcurementView");
            }

            //ProductsAuthorize
            if (Convert.ToBoolean(formCollection["ProductsAuthorize"]))
            {
                accountController.AddUserToRole(userId, "ProductsAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ProductsAuthorize");
            }

            //ProductsEdit
            if (Convert.ToBoolean(formCollection["ProductsEdit"]))
            {
                accountController.AddUserToRole(userId, "ProductsEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ProductsEdit");
            }

            //ProductsView
            if (Convert.ToBoolean(formCollection["ProductsView"]))
            {
                accountController.AddUserToRole(userId, "ProductsView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ProductsView");
            }

            //SalesAuthorize
            if (Convert.ToBoolean(formCollection["SalesAuthorize"]))
            {
                accountController.AddUserToRole(userId, "SalesAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "SalesAuthorize");
            }


            //SalesEdit
            if (Convert.ToBoolean(formCollection["SalesEdit"]))
            {
                accountController.AddUserToRole(userId, "SalesEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "SalesEdit");
            }

            //SalesView
            if (Convert.ToBoolean(formCollection["SalesView"]))
            {
                accountController.AddUserToRole(userId, "SalesView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "SalesView");
            }

            //ServicesAuthorize
            if (Convert.ToBoolean(formCollection["ServicesAuthorize"]))
            {
                accountController.AddUserToRole(userId, "ServicesAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ServicesAuthorize");
            }

            //ServicesEdit
            if (Convert.ToBoolean(formCollection["ServicesEdit"]))
            {
                accountController.AddUserToRole(userId, "ServicesEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ServicesEdit");
            }

            //ServicesView
            if (Convert.ToBoolean(formCollection["ServicesView"]))
            {
                accountController.AddUserToRole(userId, "ServicesView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "ServicesView");
            }

            //StockAuthorize
            if (Convert.ToBoolean(formCollection["StockAuthorize"]))
            {
                accountController.AddUserToRole(userId, "StockAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "StockAuthorize");
            }

            //StockEdit
            if (Convert.ToBoolean(formCollection["StockEdit"]))
            {
                accountController.AddUserToRole(userId, "StockEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "StockEdit");
            }

            //StockView
            if (Convert.ToBoolean(formCollection["StockView"]))
            {
                accountController.AddUserToRole(userId, "StockView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "StockView");
            }

            //SysAdminAuthorize
            if (Convert.ToBoolean(formCollection["SysAdminAuthorize"]))
            {
                accountController.AddUserToRole(userId, "SysAdminAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "SysAdminAuthorize");
            }

            //SysAdminEdit
            if (Convert.ToBoolean(formCollection["SysAdminEdit"]))
            {
                accountController.AddUserToRole(userId, "SysAdminEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "SysAdminEdit");
            }

            //SysAdminView
            if (Convert.ToBoolean(formCollection["SysAdminView"]))
            {
                accountController.AddUserToRole(userId, "SysAdminView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "SysAdminView");
            }

            //WorksAuthorize
            if (Convert.ToBoolean(formCollection["WorksAuthorize"]))
            {
                accountController.AddUserToRole(userId, "WorksAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "WorksAuthorize");
            }

            //WorksEdit
            if (Convert.ToBoolean(formCollection["WorksEdit"]))
            {
                accountController.AddUserToRole(userId, "WorksEdit");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "WorksEdit");
            }

            //WorksView
            if (Convert.ToBoolean(formCollection["WorksView"]))
            {
                accountController.AddUserToRole(userId, "WorksView");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "WorksView");
            }

            //WorksPlanner
            if (Convert.ToBoolean(formCollection["WorksPlanner"]))
            {
                accountController.AddUserToRole(userId, "WorksPlanner");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "WorksPlanner");
            }

            //FileAuthorize
            if (Convert.ToBoolean(formCollection["FileAuthorize"]))
            {
                accountController.AddUserToRole(userId, "FileAuthorize");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "FileAuthorize");
            }

            //FileUpload
            if (Convert.ToBoolean(formCollection["FileUpload"]))
            {
                accountController.AddUserToRole(userId, "FileUpload");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "FileUpload");
            }

            //FileDownload
            if (Convert.ToBoolean(formCollection["FileDownload"]))
            {
                accountController.AddUserToRole(userId, "FileDownload");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "FileDownload");
            }

            //FileAdmin
            if (Convert.ToBoolean(formCollection["FileAdmin"]))
            {
                accountController.AddUserToRole(userId, "FileAdmin");
            }
            else
            {
                accountController.RemoveUserToRole(userId, "FileAdmin");
            }

            return RedirectToAction("SelectUserToManageRoles");
        }

        [EsAuthorize(Roles = "OrganizationLevel")]
        [HttpGet]
        public async Task<ActionResult> SelectUserToManageRoles()
        {
            var user = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.UserId = new SelectList(await AspNetUserService.SelectAsync(user.OrgId.ToString()), "Id", "Name", user.Id);
            return View();
        }

        [EsAuthorize(Roles = "OrganizationLevel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectUserToManageRoles(string userId)
        {
            var user = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.UserId = new SelectList(await AspNetUserService.SelectAsync(user.OrgId.ToString()), "Id", "Name", user.Id);
            return RedirectToAction("ManageUserRoles", new { userId = userId });
        }


    }
}