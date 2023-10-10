using Lifetrons.Erp.Controllers;

namespace Lifetrons.Erp.Reports.Controllers
{
    using System;
    using Lifetrons.Erp.Helpers;
    using Lifetrons.Erp.Service;
    using Microsoft.AspNet.Identity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;


    public partial class SalesReportsController
    {

        #region AccountHistoryReport

        [HttpGet]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public async Task<ActionResult> ParamAccountHistory()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.AccountId = null;
            ViewBag.AccountId = new SelectList(await AccountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        [HttpPost]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public async Task<ActionResult> ParamAccountHistory(string startDate, string endDate, string accountId)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            //Get Data
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            TempData["UserId"] = applicationUser.Id;
            TempData["AccountName"] = AccountService.Find(accountId, applicationUser.Id, applicationUser.OrgId.ToString()).Name;
            var contacts = await ContactService.SelectAsyncContactsByAccount(accountId, applicationUser.OrgId.ToString());
            TempData["AccountContacts"] = await ContactService.SelectAsyncContactsByAccount(accountId, applicationUser.OrgId.ToString());
            TempData["AccountOpportunities"] = await OpportunityService.GetOpportunitiesByAccountAsync(startDate1, endDate1, accountId, applicationUser.OrgId.ToString());
            TempData["AccountQuotes"] = await QuoteService.GetQuotesByAccountAsync(startDate1, endDate1, accountId, applicationUser.OrgId.ToString());
            TempData["AccountOrders"] = await OrderService.GetOrdersByAccountAsync(startDate1, endDate1, accountId, applicationUser.OrgId.ToString());
            TempData["AccountCases"] = await CaseService.SelectAsyncCasesByAccount(startDate1, endDate1, accountId, applicationUser.OrgId.ToString());
            TempData["AccountTasks"] = await TaskService.SelectAsyncTasksByAccount(startDate1, endDate1, accountId, applicationUser.OrgId.ToString());

            return View("AccountHistoryReport");
        }


        #endregion

    }

}