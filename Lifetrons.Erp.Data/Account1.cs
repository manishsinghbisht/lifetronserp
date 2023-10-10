
using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Account")]
    [MetadataType(typeof(AccountMetadata))]
    public partial class Account : Entity
    {
       
        internal sealed class AccountMetadata
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

            public string Remark { get; set; }

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

           [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_AccountTypeId_Account_Type")]
            public Nullable<System.Guid> AccountTypeId { get; set; }
            
            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_IndustryId_Industry")]
            public Nullable<System.Guid> IndustryId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_OwnershipId_Ownership")]
            public Nullable<System.Guid> OwnershipId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_AnnualRevenue_Revenue")]
            public Nullable<decimal> AnnualRevenue { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_NumberOfEmployees_Employees")]
            public Nullable<decimal> NumberOfEmployees { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_NumberOfLocations_Locations")]
            public Nullable<decimal> NumberOfLocations { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_RatingId_Rating")]
            public Nullable<System.Guid> RatingId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_AgreementSerialNumber_Agreement_Number")]
            public string AgreementSerialNumber { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_AgreementExpDate_Agreement_Exp__Dt_")]
            [DataType(DataType.Date)]
            public System.DateTime AgreementExpDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "AccountMetadata_Priority_Priority")]
            public string Priority { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "OrderMetadata_BillingAddressId_Billing_Address")]
            public Nullable<System.Guid> BillingAddressId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressToName")]
            public string BillingAddressToName { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine1")]
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


            [Display(ResourceType = typeof(Resources.Resources), Name = "OrderMetadata_ShippingAddressId_Shipping_Address")]
            public Nullable<System.Guid> ShippingAddressId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressToName")]
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
