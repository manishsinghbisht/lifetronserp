
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Campaign")]
    [MetadataType(typeof(CampaignMetadata))]
    public partial class Campaign : Entity
    {
        internal sealed class CampaignMetadata
        {
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

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Desc_Updates")]
            public string Desc { get; set; }

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


            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_CampaignTypeId_Campaign_Type")]
            public System.Guid CampaignTypeId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_CampaignStatusId_Campaign_Status")]
            public System.Guid CampaignStatusId { get; set; }
            
            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_ParentCampaignId_Parent_Campaign")]
            public Nullable<System.Guid> ParentCampaignId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_ExpectedResponsePercent_Response_Expectation____")]
            public Nullable<decimal> ExpectedResponsePercent { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_NumSent_Target_Approach")]
            public Nullable<decimal> NumSent { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_StartDate_Start_Date")]
            public System.DateTime StartDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_EndDate_End_Date")]
            public System.DateTime EndDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_ExpectedRevenue_Expected_Revenue")]
            public Nullable<decimal> ExpectedRevenue { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_BudgetCost_Budget_Cost")]
            public Nullable<decimal> BudgetCost { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_ActualCost_Actual_Cost")]
            public Nullable<decimal> ActualCost { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "CampaignMetadata_NumberOfEmployees_Number_Of_Employees")]
            public Nullable<decimal> NumberOfEmployees { get; set; }

        }
    }
}
