
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Reports.Controllers
{
    using Lifetrons.Erp.Data;
    using Lifetrons.Erp.Helpers;
    using Microsoft.AspNet.Identity;
    using Microsoft.Reporting.WebForms;
    using System.Data;
    using System.IO;
    using System.Web.Mvc;

    public partial class SalesReportsController 
    {
        /// <summary>
        /// Order Print
        /// </summary>
        /// <param name="reportFormatType"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [Audit(AuditingLevel = 0)]
        public ActionResult Order(string reportFormatType, string orderId)
        {
            var lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/Reports/Rdlc"), "Order.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
                lr.EnableHyperlinks = true;
            }
            else
            {
                return View("Index");
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var result1 = OrderService.Select(orderId, applicationUser.Id, applicationUser.OrgId.ToString());
            var dataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(result1);
            dataTable.Columns.Add("OrganizationName", typeof(System.String));
            dataTable.Columns.Add("OrganizationSlogan", typeof(System.String));
            dataTable.Columns.Add("OrganizationWebsite", typeof(System.String));
            dataTable.Columns.Add("OrganizationAddress", typeof(System.String));
            dataTable.Columns.Add("UserEMail", typeof(System.String));
            dataTable.Columns.Add("UserPhone", typeof(System.String));
            dataTable.Columns.Add("UserFullName", typeof(System.String));

            foreach (DataRow row in dataTable.Rows)
            {
                var organization = (Organization)row["Organization"];
                row["OrganizationName"] = organization.Name;
                row["OrganizationSlogan"] = organization.Slogan;
                row["OrganizationWebsite"] = organization.Website;
                row["OrganizationAddress"] = organization.AddressLine1 + organization.AddressLine2;
                var user = (AspNetUser)row["AspNetUser2"]; //Owner
                row["UserFullName"] = user.FirstName + " " + user.LastName;
                row["UserEMail"] = user.Email;
                row["UserPhone"] = user.Mobile;
            }

            var result2 = OrderLineItemService.SelectLineItems(orderId, applicationUser.Id, applicationUser.OrgId.ToString());
            var lineItemDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(result2);
            lineItemDataTable.Columns.Add("ProductName", typeof(System.String));
            lineItemDataTable.Columns.Add("ProductCode", typeof(System.String));

            foreach (DataRow row in lineItemDataTable.Rows)
            {
                var product = (Product)row["Product"];
                row["ProductName"] = product.Name;
                row["ProductCode"] = product.Code;
            }

            //Presentable date time according to user's time zone
            dataTable = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(dataTable, User.TimeZone());
            lineItemDataTable = ControllerHelper.ChangeDateTimeInDataTableToUserTimeZone(lineItemDataTable, User.TimeZone());

            //Assign datasources
            var datasource1 = new ReportDataSource("Order", dataTable);
            lr.DataSources.Add(datasource1);

            lineItemDataTable.DefaultView.Sort = "Serial" + " " + "ASC";
            lineItemDataTable = lineItemDataTable.DefaultView.ToTable();
            var datasource2 = new ReportDataSource("OrderLineItem", lineItemDataTable);
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

    }

}