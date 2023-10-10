using System;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Case")]
    [MetadataType(typeof(CaseMetadata))]
    public partial class Case : Entity
    {
        internal sealed class CaseMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_Name_Name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_Code_Code")]
            public string Code { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_ShrtDesc_Description")]
            public string ShrtDesc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Desc_Updates")]
            public string Desc { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_OwnerId_Owner")]
            public string OwnerId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Authorized")]
            public bool Authorized { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Active")]
            public bool Active { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            public System.DateTime ModifiedDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AccountId_Account")]
            public Nullable<System.Guid> AccountId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_ContactId_Contact")]
            public Nullable<System.Guid> ContactId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_ProductId_Product")]
            public Nullable<System.Guid> ProductId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_CampaignId_Campaign")]
            public Nullable<System.Guid> CampaignId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_OpportunityId_Opportunity")]
            public Nullable<System.Guid> OpportunityId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_QuoteId_Quote")]
            public Nullable<System.Guid> QuoteId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_OrderId_Order")]
            public Nullable<System.Guid> OrderId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_InvoiceId_Invoice")]
            public Nullable<System.Guid> InvoiceId { get; set; }


            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_Subject_Subject")]
            public string Subject { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ReferenceNumber")]
            public string RefNo { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_InternalComments_Internal_Comments")]
            public string InternalComments { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_PriorityId_Priority")]
            public System.Guid PriorityId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_CaseReasonId_Reason")]
            public System.Guid CaseReasonId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_OpeningDate_Opening_Date")]
            public System.DateTime OpeningDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_Status")]
            public System.Guid CaseStatusId { get; set; }

            

            [Display(Name = "Assigned To")]
            public string AssignedToId { get; set; }

            

            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_ReportCompletionToId_Report_Completion_To")]
            public string ReportCompletionToId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_CloseDate_Close_Date")]
            public Nullable<System.DateTime> CloseDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CaseMetadata_ClosingComments_Closing_Comments")]
            public string ClosingComments { get; set; }


            
        }
    }
}
