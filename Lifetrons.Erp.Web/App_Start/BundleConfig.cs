using System.Web;
using System.Web.Optimization;

namespace Lifetrons.Erp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/bootstrap-datetimepicker.js",
                      "~/Scripts/bootstrap-maxlength.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome/css/font-awesome.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/Badger.css"
                    ));
            bundles.Add(new StyleBundle("~/Content/esCustoms").Include(
                     "~/Content/esCustoms.css"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/ESCustomScript").Include(
           "~/Scripts/ESCustomScript.js"));

            bundles.Add(new ScriptBundle("~/bundles/ExternalLoginConfirmation").Include(
          "~/Scripts/ExternalLoginConfirmation.js"));

            bundles.Add(new ScriptBundle("~/bundles/Register").Include(
           "~/Scripts/Register.js"));

            bundles.Add(new ScriptBundle("~/bundles/Home").Include(
           "~/Scripts/Home.js"));

            bundles.Add(new ScriptBundle("~/bundles/Login").Include(
           "~/Scripts/Login.js"));

            bundles.Add(new ScriptBundle("~/bundles/HideMenus").Include(
          "~/Scripts/HideMenus.js"));

            bundles.Add(new ScriptBundle("~/bundles/Email").Include(
          "~/Scripts/Email.js"));

            bundles.Add(new ScriptBundle("~/bundles/AspNetUser").Include(
         "~/Scripts/AspNetUser.js"));

            bundles.Add(new ScriptBundle("~/bundles/Organization").Include(
          "~/Scripts/Organization.js"));

            bundles.Add(new ScriptBundle("~/bundles/Department").Include(
       "~/Scripts/Department.js"));

            bundles.Add(new ScriptBundle("~/bundles/Team").Include(
        "~/Scripts/Team.js"));

            bundles.Add(new ScriptBundle("~/bundles/Hierarchy").Include(
        "~/Scripts/Hierarchy.js"));

            bundles.Add(new ScriptBundle("~/bundles/PriceBook").Include(
            "~/Scripts/PriceBook.js"));

            bundles.Add(new ScriptBundle("~/bundles/PriceBookLineItem").Include(
          "~/Scripts/PriceBookLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/Campaign").Include(
          "~/Scripts/Campaign.js"));

            bundles.Add(new ScriptBundle("~/bundles/CampaignMember").Include(
         "~/Scripts/CampaignMember.js"));

            bundles.Add(new ScriptBundle("~/bundles/Lead").Include(
            "~/Scripts/Lead.js"));

            bundles.Add(new ScriptBundle("~/bundles/SAccount").Include(
            "~/Scripts/SAccount.js"));

            bundles.Add(new ScriptBundle("~/bundles/Contact").Include(
           "~/Scripts/Contact.js"));

            bundles.Add(new ScriptBundle("~/bundles/Task").Include(
           "~/Scripts/Task.js"));

            bundles.Add(new ScriptBundle("~/bundles/Opportunity").Include(
             "~/Scripts/Opportunity.js"));

            bundles.Add(new ScriptBundle("~/bundles/OpportunityLineItem").Include(
           "~/Scripts/OpportunityLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/Quote").Include(
             "~/Scripts/Quote.js"));

            bundles.Add(new ScriptBundle("~/bundles/QuoteLineItem").Include(
            "~/Scripts/QuoteLineItem.js"));


            bundles.Add(new ScriptBundle("~/bundles/Contract").Include(
            "~/Scripts/Contract.js"));

            bundles.Add(new ScriptBundle("~/bundles/Order").Include(
           "~/Scripts/Order.js"));

            bundles.Add(new ScriptBundle("~/bundles/OrderLineItem").Include(
           "~/Scripts/OrderLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/WorkOrder").Include(
           "~/Scripts/WorkOrder.js"));

            bundles.Add(new ScriptBundle("~/bundles/Invoice").Include(
          "~/Scripts/Invoice.js"));

            bundles.Add(new ScriptBundle("~/bundles/InvoiceLineItem").Include(
           "~/Scripts/InvoiceLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/Case").Include(
         "~/Scripts/Case.js"));

            bundles.Add(new ScriptBundle("~/bundles/ImageUploader").Include(
        "~/Scripts/ImageUploader.js"));

            bundles.Add(new ScriptBundle("~/bundles/Address").Include(
                "~/Scripts/Address.js"));

            bundles.Add(new ScriptBundle("~/bundles/Product").Include(
           "~/Scripts/Product.js"));

            bundles.Add(new ScriptBundle("~/bundles/Target").Include(
          "~/Scripts/Target.js"));

            bundles.Add(new ScriptBundle("~/bundles/NoticeBoard").Include(
          "~/Scripts/NoticeBoard.js"));

            bundles.Add(new ScriptBundle("~/bundles/Audit").Include(
         "~/Scripts/Audit.js"));

            bundles.Add(new ScriptBundle("~/bundles/Report").Include(
          "~/Scripts/Report.js"));

            bundles.Add(new ScriptBundle("~/bundles/Item").Include(
          "~/Scripts/Item.js"));

            bundles.Add(new ScriptBundle("~/bundles/Process").Include(
        "~/Scripts/Process.js"));

            bundles.Add(new ScriptBundle("~/bundles/ProcessTimeConfig").Include(
       "~/Scripts/ProcessTimeConfig.js"));

            bundles.Add(new ScriptBundle("~/bundles/BOM").Include(
          "~/Scripts/BOM.js"));

            bundles.Add(new ScriptBundle("~/bundles/BOMLineItem").Include(
          "~/Scripts/BOMLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/Operation").Include(
         "~/Scripts/Operation.js"));

            bundles.Add(new ScriptBundle("~/bundles/OperationBOMLineItem").Include(
         "~/Scripts/OperationBOMLineItem.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/StockIssueHead").Include(
         "~/Scripts/StockIssueHead.js"));

            bundles.Add(new ScriptBundle("~/bundles/StockItemIssueLineItem").Include(
        "~/Scripts/StockItemIssueLineItem.js"));
            bundles.Add(new ScriptBundle("~/bundles/StockProductIssueLineItem").Include(
        "~/Scripts/StockProductIssueLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/StockReceiptHead").Include(
        "~/Scripts/StockReceiptHead.js"));

            bundles.Add(new ScriptBundle("~/bundles/StockItemReceiptLineItem").Include(
        "~/Scripts/StockItemReceiptLineItem.js"));
            bundles.Add(new ScriptBundle("~/bundles/StockProductReceiptLineItem").Include(
        "~/Scripts/StockProductReceiptLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/JobIssueHead").Include(
       "~/Scripts/JobIssueHead.js"));

            bundles.Add(new ScriptBundle("~/bundles/JobItemIssueLineItem").Include(
        "~/Scripts/JobItemIssueLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/JobProductIssueLineItem").Include(
        "~/Scripts/JobProductIssueLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/JobReceiptHead").Include(
      "~/Scripts/JobReceiptHead.js"));

            bundles.Add(new ScriptBundle("~/bundles/JobItemReceiptLineItem").Include(
        "~/Scripts/JobItemReceiptLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/JobProductReceiptLineItem").Include(
        "~/Scripts/JobProductReceiptLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/Dispatch").Include(
      "~/Scripts/Dispatch.js"));

            bundles.Add(new ScriptBundle("~/bundles/DispatchLineItem").Include(
        "~/Scripts/DispatchLineItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/Employee").Include(
     "~/Scripts/Employee.js"));

            bundles.Add(new ScriptBundle("~/bundles/ProdPlan").Include(
      "~/Scripts/ProdPlan.js"));

            bundles.Add(new ScriptBundle("~/bundles/ProdPlanDetail").Include(
      "~/Scripts/ProdPlanDetail.js"));

            bundles.Add(new ScriptBundle("~/bundles/PlanControl").Include(
     "~/Scripts/PlanControl.js"));

            bundles.Add(new ScriptBundle("~/bundles/PrintRawBooking").Include(
    "~/Scripts/PrintRawBooking.js"));

            bundles.Add(new ScriptBundle("~/bundles/ProcurementRequest").Include(
     "~/Scripts/ProcurementRequest.js"));

            bundles.Add(new ScriptBundle("~/bundles/ProcurementRequestDetail").Include(
     "~/Scripts/ProcurementRequestDetail.js"));

            bundles.Add(new ScriptBundle("~/bundles/ProcurementOrder").Include(
     "~/Scripts/ProcurementOrder.js"));

            bundles.Add(new ScriptBundle("~/bundles/ProcurementOrderDetail").Include(
     "~/Scripts/ProcurementOrderDetail.js"));

            bundles.Add(new ScriptBundle("~/bundles/PendingJobReceipts").Include(
     "~/Scripts/PendingJobReceipts.js"));

            bundles.Add(new ScriptBundle("~/bundles/Attendance").Include(
    "~/Scripts/Attendance.js"));


            bundles.Add(new ScriptBundle("~/Scripts/fancybox/fancyboxScripts").Include(
                "~/Scripts/fancybox/jquery.fancybox.js",
                "~/Scripts/fancybox/jquery.fancybox-buttons.js",
                "~/Scripts/fancybox/jquery.fancybox-thumbs.js",
                "~/Scripts/fancybox/jquery.fancybox-media.js",
                "~/Scripts/fancybox/Media.js"));

            bundles.Add(new StyleBundle("~/Scripts/fancybox/fancyboxCss").Include(
                      "~/Scripts/fancybox/jquery.fancybox.css",
                      "~/Scripts/fancybox/jquery.fancybox-buttons.css",
                      "~/Scripts/fancybox/jquery.fancybox-thumbs.css"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/File").Include(
          "~/Scripts/File.js"));

            bundles.Add(new ScriptBundle("~/bundles/FileRateTable").Include(
         "~/Scripts/FileRateTable.js"));

            bundles.Add(new ScriptBundle("~/bundles/EmailFile").Include(
                            "~/Scripts/EmailFile.js"));

            bundles.Add(new ScriptBundle("~/bundles/GeoLocation").Include(
                            "~/Scripts/GeoLocation.js"));

        }
    }
}
