using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using System;
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Order")]
    [MetadataType(typeof(OrderMetadata))]
    public partial class Order : Entity
    {
        internal sealed class OrderMetadata
        {
            [Display(Name = "Order No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal OrderNo { get; set; }

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

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_RefNo_Ref_No")]
            public string RefNo { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_OpportunityId_Opportunity")]
            public Nullable<System.Guid> OpportunityId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_QuoteId_Quote")]
            public Nullable<System.Guid> QuoteId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AccountId_Account")]
            public System.Guid AccountId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_SubAccount")]
            public Nullable<System.Guid> SubAccountId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_DeliveryTerms_Delivery_Terms")]
            public string DeliveryTerms { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_PaymentTerms_Payment_Terms")]
            public string PaymentTerms { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_SpecialTerms_Special_Terms")]
            public string SpecialTerms { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_ProgressPercent_Progress____")]
            public Nullable<decimal> ProgressPercent { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_ProgressDesc_Progress_Updates_")]
            public string ProgressDesc { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_ReportCompletionToId_On_Completion_Report_To")]
            public string ReportCompletionToId { get; set; }

           [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_ContractId_Contract")]
            public Nullable<System.Guid> ContractId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_PriorityId_Priority")]
            public System.Guid PriorityId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_DeliveryStatusId_Order_Status")]
            public System.Guid DeliveryStatusId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_StartDate_Start_Date")]
            public Nullable<System.DateTime> StartDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_ActivatedDate_Activated_Date")]
            public Nullable<System.DateTime> ActivatedDate { get; set; }

            /// <summary>
            /// Contact
            /// </summary>
            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_ActivatedById_Activated_By")]
            public string ActivatedById { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_DeliveryDate_Delivery_Date")]
            public Nullable<System.DateTime> DeliveryDate { get; set; }

            /// <summary>
            /// Contact1
            /// </summary>
            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_CompanySignedById_Company_Signed_By")]
            public string CompanySignedById { get; set; }

            /// <summary>
            /// Contact2
            /// </summary>
            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_CustSignedById_Customer_Signed_By")]
            public Nullable<System.Guid> CustSignedById { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_CustSignedByDate_Customer_Signed_Date")]
            public Nullable<System.DateTime> CustSignedByDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_BillingAddressId_Billing_Address")]
            public Nullable<System.Guid> BillingAddressId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressToName")]
            public string BillingAddressToName { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressLine1")]
            public string BillingAddressLine1 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressLine2")]
            public string BillingAddressLine2 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressLine3")]
            public string BillingAddressLine3 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressCity")]
            public string BillingAddressCity { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressPin")]
            public string BillingAddressPin { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressState")]
            public string BillingAddressState { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressCountry")]
            public string BillingAddressCountry { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_Phone")]
            public string BillingAddressPhone { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_EMail")]
            public string BillingAddressEMail { get; set; }


            [Display(ResourceType = typeof (Resources.Resources), Name = "OrderMetadata_ShippingAddressId_Shipping_Address")]
            public Nullable<System.Guid> ShippingAddressId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressToName")]
            public string ShippingAddressToName { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressLine1")]
            public string ShippingAddressLine1 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressLine2")]
            public string ShippingAddressLine2 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressLine3")]
            public string ShippingAddressLine3 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressCity")]
            public string ShippingAddressCity { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressPin")]
            public string ShippingAddressPin { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressState")]
            public string ShippingAddressState { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressCountry")]
            public string ShippingAddressCountry { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Phone")]
            public string ShippingAddressPhone { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_EMail")]
            public string ShippingAddressEMail { get; set; }

        }
    }
}
