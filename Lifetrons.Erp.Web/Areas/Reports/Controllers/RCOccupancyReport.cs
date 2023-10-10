
using Lifetrons.Erp.Controllers;
using WebGrease.Css.Extensions;

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
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public partial class SalesReportsController 
    {

        #region OccupancyReport

        [HttpGet]
        [EsAuthorize(Roles = "canEdit")]
        public async Task<ActionResult> ParamOccupancyReport(string reportName)
        {
            ViewBag.ReportName = reportName;
            await ConfigureParametersForView();

            return View();
        }

        private async System.Threading.Tasks.Task ConfigureParametersForView()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var userHierarchyEnumerable = HierarchyService.SelectUserHierarchy(applicationUser.Id,
                applicationUser.OrgId.ToString());
            var userHierarchyFirstOrDefault = userHierarchyEnumerable.FirstOrDefault();
            ViewBag.OwnerId = null;

            if (User.IsInRole("OrganizationLevel"))
            {
                ViewBag.OwnerId =
                    new SelectList(await HierarchyService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                        "UserId", "AspNetUser.Name");
            }
            else if (User.IsInRole("DepartmentLevel"))
            {
                if (userHierarchyFirstOrDefault != null)
                {
                    ViewBag.OwnerId =
                        new SelectList(
                            HierarchyService.SelectDepartmentHierarchy(applicationUser.Id,
                                applicationUser.OrgId.ToString(), userHierarchyFirstOrDefault.DepartmentId.ToString()), "UserId",
                            "AspNetUser.Name", applicationUser.Id);
                }
            }
            else if (User.IsInRole("TeamLevel"))
            {
                if (userHierarchyFirstOrDefault != null)
                {
                    ViewBag.OwnerId =
                        new SelectList(
                            HierarchyService.SelectTeamHierarchy(applicationUser.Id,
                                applicationUser.OrgId.ToString(), userHierarchyFirstOrDefault.TeamId.ToString()), "UserId",
                            "AspNetUser.Name", applicationUser.Id);
                }
            }
        }

        [HttpPost]
        [EsAuthorize(Roles = "canEdit")]
        public ActionResult ParamOccupancyReport(string reportName, string reportFormatType, string startDate, string endDate, string ownerId)
        {
            return RedirectToAction("GetReportOccupancyReport", new { reportName, reportFormatType, startDate, endDate, ownerId });
        }

        [Audit(AuditingLevel = 0)]
        public ActionResult GetReportOccupancyReport(string reportName, string reportFormatType, string startDate, string endDate, string ownerId)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), reportName);
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
            var result1 = TaskService.Select(startDate1, endDate1, ownerId, applicationUser.OrgId.ToString());

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
            var lineItemDataTable = CreateTaskDataTable(result1);

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