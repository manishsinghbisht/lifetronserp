using System.Threading.Tasks;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Lifetrons.Erp.SysAdmin.Controllers
{
    [EsAuthorize(Roles = "SuperAdmin, Support, SysAdminAuthorize, SysAdminEdit, SysAdminView")]
    public class AuditController : BaseController
    {
        //[EsAuthorize(Roles = "SuperAdmin, Support")]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [Audit]
        [Authorize]
        public ActionResult LevelZero()
        {
            return Content("Level 0 Audit Executed");
        }

        [Authorize]
        public ActionResult LevelOne()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [Audit(AuditingLevel = 1)]
        public ActionResult LevelOne(SimpleViewModel model)
        {
            return Content("Level 1 (Simple) Audit Executed");
        }

        [Authorize]
        public ActionResult LevelTwo()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [Audit(AuditingLevel = 2)]
        public ActionResult LevelTwo(IntermediateViewModel model)
        {
            return Content("Level 2 (Intermediate) Audit Executed");
        }

        [Authorize]
        public ActionResult LevelThree()
        {
            return View();
        }

        [HttpPost]
        [EsAuthorize]
        [Audit(AuditingLevel = 2)]
        public ActionResult LevelThree(AdvancedViewModel model)
        {
            return Content("Level 2 (Advanced) Audit Executed");
        }

        [EsAuthorize(Roles = "SuperAdmin, Support, SysAdminAuthorize, SysAdminEdit, SysAdminView")]
        public async Task<ActionResult> Index(int? page)
        {
            var records = new AuditingContext().AuditRecords.OrderByDescending(a => a.TimeAccessed);
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.TimeAccessed).ToPagedList(pageNumber, 50); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
            //return View(records);
        }

        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = new AuditingContext().AuditRecords.OrderByDescending(a => a.TimeAccessed);
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.UserName.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.TimeAccessed)
                    .ToPagedList(pageNumber, 50); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
        }
    }
}