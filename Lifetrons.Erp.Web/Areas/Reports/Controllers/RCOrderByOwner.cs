
using Lifetrons.Erp.Controllers;

namespace Lifetrons.Erp.Reports.Controllers
{
    using Lifetrons.Erp.Data;
    using Lifetrons.Erp.Helpers;
    using Lifetrons.Erp.Service;
    using Microsoft.AspNet.Identity;
    using Microsoft.Practices.Unity;
    using Microsoft.Reporting.WebForms;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Security.Permissions;
    using System.Text;
    using WebGrease.Css.Extensions;

    public partial class SalesReportsController 
    {

        #region OrdersByOwner

        [HttpGet]
        [EsAuthorize]
        public async Task<ActionResult> ParamOrdersByOwner()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.OwnerId = new SelectList(await AspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            return View();
        }

        [HttpPost]
        [EsAuthorize]
        public ActionResult ParamOrdersByOwner(string reportFormatType, string startDate, string endDate, string ownerId)
        {

            return RedirectToAction("GetReportOrdersByOwner", new { reportFormatType, startDate, endDate, ownerId });
        }


        [Audit(AuditingLevel = 0)]
        public ActionResult GetReportOrdersByOwner(string reportFormatType, string startDate, string endDate, string ownerId)
        {
            DateTime startDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDate1 = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());

            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "OrdersByOwner.rdlc");
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
            var result1 = OrderService.GetOrdersByOwner(startDate1, endDate1, ownerId, applicationUser.OrgId.ToString());
            
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