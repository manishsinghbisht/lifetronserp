using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using PagedList;
using Repository;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.SysAdmin.Controllers
{
    [EsAuthorize(Roles = "SuperAdmin, Support, SysAdminAuthorize, SysAdminEdit, SysAdminView")]
    public class AdminController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;


        public AdminController(IOrganizationService organizationService, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _organizationService = organizationService;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
           
        }

        // GET: /Org/
         [EsAuthorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> OrgIndex(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _organizationService.GetAsyncOrganizations(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }


         [EsAuthorize(Roles = "SuperAdmin")]
         public async Task<ActionResult> UserIndex(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _aspNetUserService.GetAsyncUsers(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.OrgId).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

         public async Task<ActionResult> SearchUser(string searchParam, int? page)
         {
             var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

             var records = await _aspNetUserService.GetAsyncUsers(applicationUser.Id, applicationUser.OrgId.ToString());
             var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
             var enumerablePage =
                  records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()))
                     .OrderBy(x => x.OrgId)
                     .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

             return View("UserIndex", enumerablePage);
         }
         public async Task<ActionResult> SearchOrg(string searchParam, int? page)
         {
             var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

             var records = await _organizationService.GetAsyncOrganizations(applicationUser.Id, applicationUser.OrgId.ToString());
             var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
             var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()))
                     .OrderBy(x => x.Name)
                     .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

             return View("OrgIndex", enumerablePage);
         }
    }
}