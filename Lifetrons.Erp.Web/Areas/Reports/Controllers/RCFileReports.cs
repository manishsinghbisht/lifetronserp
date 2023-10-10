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
    using System.Threading.Tasks;
    using System.Web.Mvc;


    public partial class SalesReportsController
    {
        #region FileReportByProcessor

        [HttpGet]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public async Task<ActionResult> ParamFileDeliveriesByProcessor()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.ProcessorId = null;
            ViewBag.ProcessorId = new SelectList(await AspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        [HttpPost]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public ActionResult ParamFileDeliveriesByProcessor(string startDate, string endDate, string processorId, string reportFormatType)
        {
            return RedirectToAction("GetFileDeliveriesByProcessor", new { startDate, endDate, processorId, reportFormatType });
        }

        [EsAuthorize(Roles = "OrganizationLevel")]
        [Audit(AuditingLevel = 0)]
        public ActionResult GetFileDeliveriesByProcessor(string startDate, string endDate, string processorId, string reportFormatType)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "FileDeliveriesByProcessor.rdlc");
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
            var result1 = FileService.FindDeliveriesByProcessor(Data.Helper.FileType.CV.ToString(), processorId, startDate1, endDate1);

            //Check if no records exists
            if (!result1.Any())
            {
                return Content("No records are available for the selected report criteria.");
            }

            //Verify records exits
            var firstOrDefault = result1.FirstOrDefault();

            //Grab objects for header creation 
            var organization = firstOrDefault.Organization;
            var owner = AspNetUserService.Find(applicationUser.Id);

            //Create header for report
            var dataTable = CreateHeaderDataTable(organization, owner);

            //Create lineItems for report
            var lineItemDataTable = CreateFileLineItemDataTable(result1);

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

        #region FileReportByDate

        [HttpGet]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public async Task<ActionResult> ParamFileDeliveriesByDate()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            return View();
        }

        [HttpPost]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public ActionResult ParamFileDeliveriesByDate(string startDate, string endDate, string reportFormatType)
        {
            return RedirectToAction("GetFileDeliveriesByDate", new { startDate, endDate, reportFormatType });
        }

        [EsAuthorize(Roles = "OrganizationLevel")]
        [Audit(AuditingLevel = 0)]
        public ActionResult GetFileDeliveriesByDate(string startDate, string endDate, string reportFormatType)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "FileDeliveriesByDate.rdlc");
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
            var result1 = FileService.FindDeliveriesByDate(Data.Helper.FileType.CV.ToString(), startDate1, endDate1, applicationUser.OrgId.ToString());

            //Check if no records exists
            if (!result1.Any())
            {
                return Content("No records are available for the selected report criteria.");
            }

            //Verify records exits
            var firstOrDefault = result1.FirstOrDefault();

            //Grab objects for header creation 
            var organization = firstOrDefault.Organization;
            var owner = AspNetUserService.Find(applicationUser.Id);

            //Create header for report
            var dataTable = CreateHeaderDataTable(organization, owner);

            //Create lineItems for report
            var lineItemDataTable = CreateFileLineItemDataTable(result1);

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

        #region FileReportForAccount
        [HttpGet]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public async Task<ActionResult> ParamFileReportForAccount()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.AccountId = null;
            ViewBag.AccountId = new SelectList(await AccountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        [HttpPost]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public ActionResult ParamFileReportForAccount(string startDate, string endDate, string accountId, string reportFormatType)
        {
            return RedirectToAction("GetReportFileReportForAccount", new { startDate, endDate, accountId, reportFormatType });
        }

        [EsAuthorize(Roles = "OrganizationLevel")]
        [Audit(AuditingLevel = 0)]
        public ActionResult GetReportFileReportForAccount(string startDate, string endDate, string accountId, string reportFormatType)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "FileReportForAccount.rdlc");
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
            var result1 = FileService.FindByAccount(accountId, startDate1, endDate1, Data.Helper.FileStatus.Delivered.ToString());

            //Check if no records exists
            if (!result1.Any())
            {
                return Content("No records are available for the selected report criteria.");
            }

            //Verify records exits
            var firstOrDefault = result1.FirstOrDefault();

            //Grab objects for header creation 
            var organization = firstOrDefault.Organization;
            var owner = AspNetUserService.Find(applicationUser.Id);

            //Create header for report
            var dataTable = CreateHeaderDataTable(organization, owner);

            //Create lineItems for report
            var lineItemDataTable = CreateFileLineItemDataTable(result1);

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