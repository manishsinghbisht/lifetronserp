
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

        #region OrdersByOrg

        [HttpGet]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public async Task<ActionResult> ParamOrdersByOrg()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            return View();
        }

        [HttpPost]
        [EsAuthorize(Roles = "OrganizationLevel")]
        public ActionResult ParamOrdersByOrg(string reportFormatType, string startDate, string endDate)
        {

            return RedirectToAction("GetReportOrdersByOrg", new { reportFormatType, startDate, endDate });
        }

        [Audit(AuditingLevel = 0)]
        public ActionResult GetReportOrdersByOrg(string reportFormatType, string startDate, string endDate)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "OrdersByOrg.rdlc");
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
            var result1 = OrderService.GetOrdersByOrganization(startDate1, endDate1, applicationUser.Id, applicationUser.OrgId.ToString());

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