
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Quote")]
    [MetadataType(typeof(QuoteMetadata))]
    public partial class Quote : Entity
    {
        internal sealed class QuoteMetadata
        {
            [Display(Name = "Quote No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal QuoteNo { get; set; }
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

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AccountId_Account")]
            public Nullable<System.Guid> AccountId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ContactId_Contact")]
            public Nullable<System.Guid> ContactId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_OpportunityId_Opportunity")]
            public Nullable<System.Guid> OpportunityId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ReferenceNumber")]
            public string RefNo { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Status")]
            public System.Guid QuoteStatusId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "QuoteMetadata_FollowUpDate_Follow_up_Date")]
            public Nullable<System.DateTime> FollowUpDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "QuoteMetadata_ExpirationDate_Expiry_Date")]
            public Nullable<System.DateTime> ExpirationDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "QuoteMetadata_BillingAddressId_Billing_Address")]
            public Nullable<System.Guid> BillingAddressId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "QuoteMetadata_BillingAddressToName_Bill_To_Name")]
            public string BillingAddressToName { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressLine1")]
            public string BillingAddressLine1 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine2")]
            public string BillingAddressLine2 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine3")]
            public string BillingAddressLine3 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCity")]
            public string BillingAddressCity { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressPin")]
            public string BillingAddressPin { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressState")]
            public string BillingAddressState { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCountry")]
            public string BillingAddressCountry { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Phone")]
            public string BillingAddressPhone { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_EMail")]
            public string BillingAddressEMail { get; set; }


            [Display(ResourceType = typeof (Resources.Resources), Name = "QuoteMetadata_ShippingAddressId_Shipping_Address")]
            public Nullable<System.Guid> ShippingAddressId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "QuoteMetadata_ShippingAddressToName_Ship_To_Name")]
            public string ShippingAddressToName { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine1")]
            public string ShippingAddressLine1 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine2")]
            public string ShippingAddressLine2 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine3")]
            public string ShippingAddressLine3 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCity")]
            public string ShippingAddressCity { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressPin")]
            public string ShippingAddressPin { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressState")]
            public string ShippingAddressState { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCountry")]
            public string ShippingAddressCountry { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Phone")]
            public string ShippingAddressPhone { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_EMail")]
            public string ShippingAddressEMail { get; set; }

        }
    }
}
