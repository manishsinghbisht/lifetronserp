
using System.Data;
using Lifetrons.Erp.Controllers;

namespace Lifetrons.Erp.Reports.Controllers
{
    using Lifetrons.Erp.Data;
    using Lifetrons.Erp.Helpers;
    using Lifetrons.Erp.Service;
    using Microsoft.AspNet.Identity;
    using Microsoft.Reporting.WebForms;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using WebGrease.Css.Extensions;

    public partial class SalesReportsController 
    {
        
        #region OrdersByTeam

        [HttpGet]
        [EsAuthorize(Roles = "DepartmentLevel, OrganizationLevel, TeamLevel")]
        public async Task<ActionResult> ParamOrdersByTeam()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
           
            if (User.IsInRole("OrganizationLevel"))
            {
                ViewBag.TeamId = new SelectList(await TeamService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            }
            else if (User.IsInRole("DepartmentLevel"))
            {
                var userHierarchy = HierarchyService.SelectUserHierarchy(applicationUser.Id, applicationUser.OrgId.ToString());
                var firstOrDefault = userHierarchy.FirstOrDefault();

                if (firstOrDefault != null)
                {
                    ViewBag.TeamId = new SelectList(await TeamService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString(), firstOrDefault.DepartmentId.ToString()), "Id", "Name");
                    ViewBag.ApplicationUsersTeamName = firstOrDefault.Team.Name;
                    ViewBag.ApplicationUsersTeamId = firstOrDefault.TeamId;
                }
                else
                {
                    ViewBag.ApplicationUsersDepartmentName = "Department not defined for the user in Hierarchy";
                    ViewBag.ApplicationUsersDepartmentId = null;
                }
            }
            else if (User.IsInRole("TeamLevel"))
            {
                var userHierarchy = HierarchyService.SelectUserHierarchy(applicationUser.Id, applicationUser.OrgId.ToString());
                var firstOrDefault = userHierarchy.FirstOrDefault();

                if (firstOrDefault != null)
                {
                    //Pickup from hidden field. No dropdowns
                    ViewBag.ApplicationUsersTeamName = firstOrDefault.Team.Name;
                    ViewBag.ApplicationUsersTeamId = firstOrDefault.TeamId;
                }
                else
                {
                    ViewBag.ApplicationUsersDepartmentName = "Department not defined for the user in Hierarchy";
                    ViewBag.ApplicationUsersDepartmentId = null;
                }
            }

            return View();
        }

        [HttpPost]
        [EsAuthorize(Roles = "DepartmentLevel, OrganizationLevel, TeamLevel")]
        public ActionResult ParamOrdersByTeam(string reportFormatType, string startDate, string endDate, string teamId)
        {
            return RedirectToAction("GetReportOrdersByTeam", new { reportFormatType, startDate, endDate, teamId });
        }


        [Audit(AuditingLevel = 0)]
        public ActionResult GetReportOrdersByTeam(string reportFormatType, string startDate, string endDate, string teamId)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "OrdersByTeam.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
                lr.EnableHyperlinks = true;
            }
            else
            {
                return null;
            }

            //Get Data
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var teamHierarchy = HierarchyService.SelectTeamHierarchy(applicationUser.Id, applicationUser.OrgId.ToString(), teamId);
            StringBuilder usersStringBuilder = new StringBuilder();
            teamHierarchy.ForEach(p => usersStringBuilder.Append(p.UserId + ";"));

            var result1 = OrderService.GetOrdersByUserList(startDate1, endDate1, usersStringBuilder.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            //Check if no records exists
            if (!result1.Any())
            {
                return Content("No records are available for the selected report criteria.");
            }

            //Verify records exits
            var firstOrDefault = result1.FirstOrDefault();          

            //Grab objects for header creation 
            var organization = firstOrDefault.Organization;
            var owner = firstOrDefault.AspNetUser2;

            //Create header for report
            var dataTable = CreateHeaderDataTable(organization, owner);

            //Create lineItems for report
            var lineItemDataTable = CreateOrderLineItemDataTable(result1);

            //Presentable date time according to user's time zone
            dataTable = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(dataTable, User.TimeZone());
            lineItemDataTable = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(lineItemDataTable, User.TimeZone());

            //Assign Datasources
            var datasource1 = new ReportDataSource("ReportHeader", dataTable);
            lr.DataSources.Add(datasource1);
            var datasource2 = new ReportDataSource("ReportLineItems", lineItemDataTable);
            lr.DataSources.Add(datasource2);

            //Render
            byte[] renderedBytes;
            var mimeType = FormatAndRenderBytes(reportFormatType, lr, out renderedBytes);

            return File(renderedBytes, mimeType);
        }

       #endregion

    }

}