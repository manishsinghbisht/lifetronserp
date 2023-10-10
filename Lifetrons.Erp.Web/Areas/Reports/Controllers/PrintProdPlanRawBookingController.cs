

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lifetrons.Erp.Controllers;

namespace Lifetrons.Erp.Reports.Controllers
{
    using System;
    using Lifetrons.Erp.Data;
    using Lifetrons.Erp.Helpers;
    using Microsoft.Reporting.WebForms;
    using System.Data;
    using System.IO;
    using System.Web.Mvc;
    using Lifetrons.Erp.Service;
    using Microsoft.Practices.Unity;
    using System.Data;
    using System.Web.Mvc;
    using Microsoft.Ajax.Utilities;
    using Microsoft.AspNet.Identity;
    using Microsoft.Reporting.WebForms;
    using System.Web;

    [EsAuthorize]
    public class PrintProdPlanRawBookingController : BaseController
    {
        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IProdPlanRawBookingService ProdPlanRawBookingService { get; set; }


        [Dependency]
        public IProdPlanDetailService ProdPlanDetailService { get; set; }

        [HttpGet]
        public async Task<ActionResult> Print()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            return View("../StockReports/PrintRawBooking");
        }

        /// <summary>
        /// Order Print
        /// </summary>
        /// <param name="reportFormatType"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [Audit(AuditingLevel = 0)]
        [HttpPost]
        public ActionResult Print(string reportFormatType, DateTime startDate, DateTime endDate)
        {
            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "RawMaterialBooking.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
                lr.EnableHyperlinks = true;
            }
            else
            {
                return View("Error");
            }

            //Get Data
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var headData = ProdPlanRawBookingService.SelectLineItems(startDate, endDate, applicationUser.Id, applicationUser.OrgId.ToString());

            //Check if no records exists
            if (!headData.Any())
            {
                return Content("No records are available for the selected report criteria.");
            }

            //Create header for report
            var headDataTable = CreateHeadDataTable(headData, startDate, endDate);

            //Create lineItems for report
            var detailsData = ProdPlanRawBookingService.SelectLineItems(startDate, endDate, applicationUser.Id, applicationUser.OrgId.ToString());
            var detailDataTable = CreateDetailDataTable(detailsData);


            //Presentable date time according to user's time zone
            headDataTable = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(headDataTable, User.TimeZone());
            detailDataTable = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(detailDataTable, User.TimeZone());

            //Assign datasources
            var datasource1 = new ReportDataSource("Head", headDataTable);
            lr.DataSources.Add(datasource1);

            detailDataTable.DefaultView.Sort = "Serial" + " " + "ASC";
            detailDataTable = detailDataTable.DefaultView.ToTable();
            var datasource2 = new ReportDataSource("Detail", detailDataTable);
            lr.DataSources.Add(datasource2);

            string reportType = reportFormatType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
            "  <OutputFormat>" + reportFormatType + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType);
        }

        private DataTable CreateHeadDataTable(IEnumerable<ProdPlanRawBooking> headData, DateTime startDate, DateTime endDate)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //Verify records exits
            var firstOrDefault = headData.FirstOrDefault();
            //Grab objects for header creation 
            var organization = firstOrDefault.Organization;
            var aspnetUser = firstOrDefault.AspNetUser1;

            var headDataTable = new DataTable("Head");

            //Custom column
            headDataTable.Columns.Add("StartDate", typeof(System.DateTime));
            headDataTable.Columns.Add("EndDate", typeof(System.DateTime));

            //Add standard columns
            headDataTable.Columns.Add("OrganizationName", typeof(System.String));
            headDataTable.Columns.Add("OrganizationSlogan", typeof(System.String));
            headDataTable.Columns.Add("OrganizationWebsite", typeof(System.String));
            headDataTable.Columns.Add("OrganizationAddress", typeof(System.String));
            headDataTable.Columns.Add("UserEMail", typeof(System.String));
            headDataTable.Columns.Add("UserPhone", typeof(System.String));
            headDataTable.Columns.Add("UserFullName", typeof(System.String));

            DataRow row = headDataTable.NewRow();

            //Custom Column
            row["StartDate"] = startDate;
            row["EndDate"] = endDate;

            //Fill data in standard columns
            row["OrganizationName"] = organization.Name;
            row["OrganizationSlogan"] = organization.Slogan;
            row["OrganizationWebsite"] = organization.Website;
            row["OrganizationAddress"] = organization.AddressLine1 + " " + organization.AddressLine2 + ", " + organization.City;
            row["UserFullName"] = aspnetUser.FirstName + " " + aspnetUser.LastName;
            row["UserEMail"] = aspnetUser.Email;
            row["UserPhone"] = aspnetUser.Mobile;

            headDataTable.Rows.Add(row);
            headDataTable.AcceptChanges();

            return headDataTable;
        }

        private DataTable CreateDetailDataTable(IEnumerable<ProdPlanRawBooking> detailData)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var detailDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(detailData);

            //// Do the addition of custom columns here
            detailDataTable.Columns.Add("CreatedByName", typeof(System.String));
            detailDataTable.Columns.Add("Serial", typeof(System.Int32));
            detailDataTable.Columns.Add("WeightUOMName", typeof(System.String));
            detailDataTable.Columns.Add("ItemName", typeof(System.String));
            detailDataTable.Columns.Add("ItemCode", typeof(System.String));
            detailDataTable.Columns.Add("RequiredOnDate", typeof(System.DateTime));

            Int32 count = 1;
            foreach (DataRow row in detailDataTable.Rows)
            {
                row["Serial"] = count++;

                var prodPlanDetailId = (Guid)row["ProdPlanDetailId"];
                row["RequiredOnDate"] = ProdPlanDetailService.Find(prodPlanDetailId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString()).StartDateTime;

                var aspnetUser = (AspNetUser)row["AspNetUser"];
                row["CreatedByName"] = aspnetUser.Name;

                if (row["Item"] != System.DBNull.Value)
                {
                    var item = (Item)row["Item"];
                    row["ItemName"] = item.Name;
                    row["ItemCode"] = item.Code;
                }

                if (row["WeightUnit"] != System.DBNull.Value)
                {
                    var weightUnit = (WeightUnit)row["WeightUnit"];
                    row["WeightUOMName"] = weightUnit.Name;
                }
            }

            return detailDataTable;
        }
    }

}