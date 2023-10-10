using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Opportunity")]
    [MetadataType(typeof(OpportunityMetadata))]
    public partial class Opportunity : Entity
    {
        internal sealed class OpportunityMetadata
        {
            [Display(Name = "Opportunity No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal OpportunityNo { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Name_Name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Code_Code")]
            public string Code { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ShrtDesc_Description")]
            public string ShrtDesc { get; set; }

           
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_OwnerId_Owner")]
            public string OwnerId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Authorized")]
            public bool Authorized { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Active")]
            public bool Active { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            public System.DateTime ModifiedDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_Private_Private")]
            public bool Private { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AccountId_Account")]
            public Nullable<System.Guid> AccountId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_OpportunityTypeId_Type")]
            public System.Guid OpportunityTypeId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_LeadSourceId_Lead_Source")]
            public System.Guid LeadSourceId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_NumberOfEmployees_Number_Of_Employees")]
            public Nullable<decimal> NumberOfEmployees { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_ExpectedRevenue_Expected_Revenue")]
            public Nullable<decimal> ExpectedRevenue { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_CloseDate_Close_date")]
            public System.DateTime CloseDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_NextStep_Next_Step")]
            public string NextStep { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_StageId_Stage")]
            public System.Guid StageId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_ProbabilityPercent_Probability____")]
            public Nullable<decimal> ProbabilityPercent { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_DeliveryStatusId_Order_Status")]
            public Nullable<System.Guid> DeliveryStatusId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CampaignId_Campaign")]
            public Nullable<System.Guid> CampaignId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_LeadId_Lead")]
            public Nullable<System.Guid> LeadId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ContactId_Contact")]
            public Nullable<System.Guid> ContactId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_Competitors_Competitors")]
            public string Competitors { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ReferenceNumber")]
            public string RefNo { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OpportunityMetadata_OrderNo_Order_No")]
            public string OrderNo { get; set; }
        }
    }
}
