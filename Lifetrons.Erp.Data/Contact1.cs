
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contact")]
    [MetadataType(typeof(ContactMetadata))]
    public partial class Contact : Entity
    {
        internal sealed class ContactMetadata
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
            public string Remark { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_OwnerId_Owner")]
            public string OwnerId { get; set; }

            public string PreferredOwnerId { get; set; }

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


            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_NamePrefix_Prefix")]
            public string NamePrefix { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AccountId_Account")]
            public Nullable<System.Guid> AccountId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_MiddleName_Middle_Name")]
            public string MiddleName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_LastName_Last_Name")]
            public string LastName { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_LevelId_Level")]
            public System.Guid LevelId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_PrimaryPhone_Primary_Phone")]
            public string PrimaryPhone { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_PrimaryEMail_Primary_Email")]
            public string PrimaryEMail { get; set; }
           
            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_CompanyName_Company")]
            public string CompanyName { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_LeadSourceId_Lead_Source")]
            public System.Guid LeadSourceId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_Birthdate")]
            public System.DateTime Birthdate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_IsProspect_Is_Prospect")]
            public bool IsProspect { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_IsEndorsement_Is_Endorsement")]
            public bool IsEndorsement { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_MailingAddressId_Mailing_Address")]
            public Nullable<System.Guid> MailingAddressId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressToName")]
            public string MailingAddressToName { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine1")]
            public string MailingAddressLine1 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine2")]
            public string MailingAddressLine2 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine3")]
            public string MailingAddressLine3 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCity")]
            public string MailingAddressCity { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressPin")]
            public string MailingAddressPin { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressState")]
            public string MailingAddressState { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCountry")]
            public string MailingAddressCountry { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Phone")]
            public string MailingAddressPhone { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_EMail")]
            public string MailingAddressEMail { get; set; }


            [Display(ResourceType = typeof (Resources.Resources), Name = "ContactMetadata_OtherAddressId_Other_Address")]
            public Nullable<System.Guid> OtherAddressId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressToName")]
            public string OtherAddressToName { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine1")]
            public string OtherAddressLine1 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine2")]
            public string OtherAddressLine2 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine3")]
            public string OtherAddressLine3 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCity")]
            public string OtherAddressCity { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressPin")]
            public string OtherAddressPin { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressState")]
            public string OtherAddressState { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCountry")]
            public string OtherAddressCountry { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Phone")]
            public string OtherAddressPhone { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_EMail")]
            public string OtherAddressEMail { get; set; }


        }
    }
}
