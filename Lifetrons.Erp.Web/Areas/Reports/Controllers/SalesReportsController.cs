

using Lifetrons.Erp.Controllers;

namespace Lifetrons.Erp.Reports.Controllers
{
    using System;
    using Lifetrons.Erp.Helpers;
    using Lifetrons.Erp.Data;
    using Lifetrons.Erp.Service;
    using Microsoft.Practices.Unity;
    using Microsoft.Reporting.WebForms;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.Mvc;
    using Microsoft.Ajax.Utilities;
    using Microsoft.AspNet.Identity;

    [EsAuthorize]
    public partial class SalesReportsController : BaseController
    {
        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IAccountService AccountService { get; set; }

        [Dependency]
        public IContactService ContactService { get; set; }

        [Dependency]
        public ITaskService TaskService { get; set; }

        [Dependency]
        public IOpportunityService OpportunityService { get; set; }

        [Dependency]
        public IQuoteService QuoteService { get; set; }

        [Dependency]
        public IQuoteLineItemService QuoteLineItemService { get; set; }

        [Dependency]
        public IOrderService OrderService { get; set; }

        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }

        [Dependency]
        public IInvoiceService InvoiceService { get; set; }

        [Dependency]
        public IInvoiceLineItemService InvoiceLineItemService { get; set; }

        [Dependency]
        public ICaseService CaseService { get; set; }
       
        [Dependency]
        public IHierarchyService HierarchyService { get; set; }

        [Dependency]
        public IDepartmentService DepartmentService { get; set; }

        [Dependency]
        public ITeamService TeamService { get; set; }

        [Dependency]
        public IFileService FileService { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        #region private methods
        public static DataTable CreateHeaderDataTable(Organization organization, AspNetUser owner)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("OrganizationName", typeof(System.String));
            dataTable.Columns.Add("OrganizationSlogan", typeof(System.String));
            dataTable.Columns.Add("OrganizationWebsite", typeof(System.String));
            dataTable.Columns.Add("OrganizationAddress", typeof(System.String));
            dataTable.Columns.Add("UserEMail", typeof(System.String));
            dataTable.Columns.Add("UserPhone", typeof(System.String));
            dataTable.Columns.Add("UserFullName", typeof(System.String));

            DataRow headRow = dataTable.NewRow();
            headRow["OrganizationName"] = organization.Name;
            headRow["OrganizationSlogan"] = organization.Slogan;
            headRow["OrganizationWebsite"] = organization.Website;
            headRow["OrganizationAddress"] = organization.AddressLine1 + (organization.AddressLine2.IsNullOrWhiteSpace() ? "" : ", " + organization.AddressLine2) + ", " + organization.City + " - " + organization.PostalCode + ", " + organization.State + ", " + organization.Country;
            headRow["UserFullName"] = owner.FirstName + " " + owner.LastName;
            headRow["UserEMail"] = owner.Email;
            headRow["UserPhone"] = owner.Mobile;
            dataTable.Rows.Add(headRow);
            return dataTable;
        }
        private static DataTable CreateOrderLineItemDataTable(IEnumerable<Order> result1)
        {
            var lineItemDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(result1);

            lineItemDataTable.Columns.Add("Owner", typeof (System.String));
            lineItemDataTable.Columns.Add("AccountName", typeof (System.String));
            lineItemDataTable.Columns.Add("DeliveryStatusName", typeof (System.String));

            foreach (DataRow row in lineItemDataTable.Rows)
            {
                var account = (Account) row["Account"]; //Account
                var deliveryStatus = (DeliveryStatu) row["DeliveryStatu"]; //Delivery Status
                var owner = (AspNetUser) row["AspNetUser2"];
                row["Owner"] = owner.UserName;
                row["AccountName"] = account.Name;
                row["DeliveryStatusName"] = deliveryStatus.Name;
            }
            return lineItemDataTable;
        }

        private static DataTable CreateFileLineItemDataTable(IEnumerable<File> result1)
        {
            var lineItemDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(result1);

            lineItemDataTable.Columns.Add("ProcessorName", typeof(System.String));
            lineItemDataTable.Columns.Add("ContactName", typeof(System.String));
            lineItemDataTable.Columns.Add("AccountName", typeof(System.String));
            lineItemDataTable.Columns.Add("Amount", typeof(System.Double));

            foreach (DataRow row in lineItemDataTable.Rows)
            {
                var contact = (Contact)row["Contact"]; //Contact
                var account = (Account)row["Account"]; //Account
                var processor = (AspNetUser)row["AspNetUserProcessor"];
                row["ProcessorName"] = processor.UserName;
                row["ContactName"] = contact.Name;
                row["AccountName"] = account.Name;

                if (row["RateType"].ToString() == Data.Helper.FileRateType.PerPageUrgent.ToString() ||
                    row["RateType"].ToString() == Data.Helper.FileRateType.PerPageNonUrgent.ToString())
                {
                    row["Amount"] = Convert.ToDouble(row["Rate"]) * Convert.ToDouble(row["NumberOfPagesSubmitted"]);
                }
                else if (row["RateType"].ToString() == Data.Helper.FileRateType.PerFileUrgent.ToString() ||
                    row["RateType"].ToString() == Data.Helper.FileRateType.PerFileNonUrgent.ToString())
                {
                    row["Amount"] = Convert.ToDouble(row["Rate"]);
                }
            }

            return lineItemDataTable;
        }

        private DataTable CreateTaskDataTable(IEnumerable<Lifetrons.Erp.Data.Task> result1)
        {
            var currentApplicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var lineItemDataTable = Lifetrons.Erp.Data.ExtensionMethods.ToDataTable(result1);
            lineItemDataTable.DefaultView.Sort = "StartDate";

            lineItemDataTable.Columns.Add("LeadName", typeof(System.String));
            lineItemDataTable.Columns.Add("ContactName", typeof(System.String));
            lineItemDataTable.Columns.Add("Owner", typeof(System.String));
            lineItemDataTable.Columns.Add("StatusName", typeof(System.String));
            lineItemDataTable.Columns.Add("RelatedToName", typeof(System.String));

            foreach (DataRow row in lineItemDataTable.Rows)
            {
                var leadName = row["Lead"] == System.DBNull.Value ? string.Empty : ((Lead)row["Lead"]).Name; //Lead
                var contactName = row["Contact"] == System.DBNull.Value ? string.Empty : ((Contact)row["Contact"]).Name; //Contact
                var statusName = row["TaskStatu"] == System.DBNull.Value ? string.Empty : ((TaskStatu)row["TaskStatu"]).Name;  //Status
                var owner = (AspNetUser)row["AspNetUser2"];
                row["LeadName"] = leadName;
                row["ContactName"] = contactName;
                row["Owner"] = owner.UserName;
                row["StatusName"] = statusName;
                row["RelatedToName"] = TaskService.GetRelatedToIdName(Convert.ToString(row["RelatedToObjectName"]), Convert.ToString(row["RelatedToId"]), currentApplicationUser.Id, currentApplicationUser.OrgId.ToString());
            }

            return lineItemDataTable;
        }

        private static string FormatAndRenderBytes(string reportFormatType, LocalReport lr, out byte[] renderedBytes)
        {
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

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return mimeType;
        }
        #endregion

    }

}