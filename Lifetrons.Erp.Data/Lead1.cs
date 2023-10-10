using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using System;
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Lead")]
    [MetadataType(typeof(LeadMetadata))]
    public partial class Lead : Entity
    {
        internal sealed class LeadMetadata
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

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            public System.DateTime ModifiedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Authorized")]
            public bool Authorized { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Active")]
            public bool Active { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_PrimaryPhone_Primary_Phone")]
            public string PrimaryPhone { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_PrimaryEMail_Primary_Email")]
            public string PrimaryEMail { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_CompanyName_Company")]
            public string CompanyName { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_Department_Department")]
            public string Department { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_Title_Designation")]
            public string Title { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AnnualRevenue_Annual_Revenue")]
            public Nullable<decimal> AnnualRevenue { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_NumberOfEmployees_Number_Of_Employees")]
            public Nullable<decimal> NumberOfEmployees { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_LeadSourceId_Lead_Source")]
            public System.Guid LeadSourceId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_LeadStatusId_Lead_Status")]
            public System.Guid LeadStatusId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_RatingId_Rating")]
            public Nullable<System.Guid> RatingId { get; set; }

            
            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_CampaignId_Campaign")]
            public Nullable<System.Guid> CampaignId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_IsConverted_Converted")]
            public bool IsConverted { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressId_Address")]
            public Nullable<System.Guid> AddressId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressToName_Address_To_Name")]
            public string AddressToName { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressLine1_Address_Line_1")]
            public string AddressLine1 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressLine2_Address_Line_2")]
            public string AddressLine2 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressLine3_Address_Line_3")]
            public string AddressLine3 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressCity_City")]
            public string AddressCity { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressPin_PIN")]
            public string AddressPin { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressState_State")]
            public string AddressState { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressCountry_Country")]
            public string AddressCountry { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressPhone_Address_Phone")]
            public string AddressPhone { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "LeadMetadata_AddressEMail_Address_E_Mail")]
            public string AddressEMail { get; set; }

        }
    }
}
