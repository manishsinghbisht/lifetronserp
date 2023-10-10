

using System.Collections.Generic;
using System.Linq;
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

    public class PrintJobReceiptController : BaseController
    {
        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IJobReceiptHeadService JobReceiptHeadService { get; set; }

        [Dependency]
        public IJobProductReceiptService JobProductReceiptService { get; set; }

        [Dependency]
        public IJobItemReceiptService JobItemReceiptService { get; set; }

        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IEmployeeService EmployeeService { get; set; }


        /// <summary>
        /// Order Print
        /// </summary>
        /// <param name="reportFormatType"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [Audit(AuditingLevel = 0)]
        public ActionResult Print(string reportFormatType, string id)
        {
            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "JobReceipt.rdlc");
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
            var headData = JobReceiptHeadService.Select(id, applicationUser.Id, applicationUser.OrgId.ToString());

            //Check if no records exists
            if (!headData.Any())
            {
                return Content("No records are available for the selected report criteria.");
            }

            //Create header for report
            var headDataTable = CreateHeadDataTable(headData);

            //Create product lineItems for report
            var detailsData1 = JobProductReceiptService.SelectLineItems(id, applicationUser.Id, applicationUser.OrgId.ToString());
            var detailDataTable1 = CreateDetailDataTable(detailsData1);

            //Create item lineItems for report
            var detailsData2 = JobItemReceiptService.SelectLineItems(id, applicationUser.Id, applicationUser.OrgId.ToString());
            var detailDataTable2 = CreateDetailDataTable(detailsData2);

            //Presentable date time according to user's time zone
            headDataTable = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(headDataTable, User.TimeZone());
            detailDataTable1 = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(detailDataTable1, User.TimeZone());
            detailDataTable2 = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(detailDataTable2, User.TimeZone());

            //Assign datasources
            var datasource1 = new ReportDataSource("Head", headDataTable);
            lr.DataSources.Add(datasource1);

            detailDataTable1.DefaultView.Sort = "Serial" + " " + "ASC";
            detailDataTable1 = detailDataTable1.DefaultView.ToTable();
            var datasource2 = new ReportDataSource("Detail1", detailDataTable1);
            lr.DataSources.Add(datasource2);

            detailDataTable2.DefaultView.Sort = "Serial" + " " + "ASC";
            detailDataTable2 = detailDataTable2.DefaultView.ToTable();
            var datasource3 = new ReportDataSource("Detail2", detailDataTable2);
            lr.DataSources.Add(datasource3);

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

        private DataTable CreateHeadDataTable(IEnumerable<JobReceiptHead> headData)
        {
            //Verify records exits
            var firstOrDefault = headData.FirstOrDefault();
            //Grab objects for header creation 
            var organization = firstOrDefault.Organization;
            var aspnetUser = firstOrDefault.AspNetUser1;
            var employee = firstOrDefault.Employee;
            var processReceiptBy = firstOrDefault.Process; //ReceiptByProcess
            var ProcessReceiptFrom = firstOrDefault.Process1; //ReceiptFromProcess

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var headDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(headData);
            //Add custom columns
            headDataTable.Columns.Add("ReceiptByProcessName", typeof(System.String));
            headDataTable.Columns.Add("ReceiptFromProcessName", typeof(System.String));
            headDataTable.Columns.Add("EmployeeName", typeof(System.String));

            //Add standard columns
            headDataTable.Columns.Add("OrganizationName", typeof(System.String));
            headDataTable.Columns.Add("OrganizationSlogan", typeof(System.String));
            headDataTable.Columns.Add("OrganizationWebsite", typeof(System.String));
            headDataTable.Columns.Add("OrganizationAddress", typeof(System.String));
            headDataTable.Columns.Add("UserEMail", typeof(System.String));
            headDataTable.Columns.Add("UserPhone", typeof(System.String));
            headDataTable.Columns.Add("UserFullName", typeof(System.String));

            foreach (DataRow row in headDataTable.Rows)
            {
                //Fill data in custom columns
                row["ReceiptByProcessName"] = processReceiptBy.Name;
                row["ReceiptFromProcessName"] = ProcessReceiptFrom.Name;
                row["EmployeeName"] = employee == null ? "" : employee.Name;

                //Fill data in standard columns
                row["OrganizationName"] = organization.Name;
                row["OrganizationSlogan"] = organization.Slogan;
                row["OrganizationWebsite"] = organization.Website;
                row["OrganizationAddress"] = organization.AddressLine1 + " " + organization.AddressLine2 + ", " + organization.City;
                row["UserFullName"] = aspnetUser.FirstName + " " + aspnetUser.LastName;
                row["UserEMail"] = aspnetUser.Email;
                row["UserPhone"] = aspnetUser.Mobile;
            }

            return headDataTable;
        }

        private DataTable CreateDetailDataTable(IEnumerable<JobProductReceipt> detailData)
        {
            var detailDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(detailData);

            //// Do the addition of custom columns here
            detailDataTable.Columns.Add("WeightUOMName", typeof(System.String));
            detailDataTable.Columns.Add("Name", typeof(System.String));
            detailDataTable.Columns.Add("Code", typeof(System.String));

            foreach (DataRow row in detailDataTable.Rows)
            {
                if (row["WeightUnit"] != System.DBNull.Value)
                {
                    var weightUnit = (WeightUnit)row["WeightUnit"];
                    row["WeightUOMName"] = weightUnit.Name;
                }
            }

            return detailDataTable;
        }

        private DataTable CreateDetailDataTable(IEnumerable<JobItemReceipt> detailData)
        {
            var detailDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(detailData);

            //// Do the addition of custom columns here
            detailDataTable.Columns.Add("WeightUOMName", typeof(System.String));
            detailDataTable.Columns.Add("Name", typeof(System.String));
            detailDataTable.Columns.Add("Code", typeof(System.String));

            foreach (DataRow row in detailDataTable.Rows)
            {

                if (row["Item"] != System.DBNull.Value)
                {
                    var item = (Item)row["Item"];
                    row["Name"] = item.Name;
                    row["Code"] = item.Code;
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