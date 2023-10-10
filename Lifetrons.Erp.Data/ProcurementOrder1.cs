
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProcurementOrder")]
    [MetadataType(typeof(ProcurementOrderMetadata))]
    public partial class ProcurementOrder : Entity
    {
        internal sealed class ProcurementOrderMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Date")]
            public System.DateTime Date { get; set; }

            [Display(Name = "Supplier Account")]
            public System.Guid AccountId { get; set; }

            [Display(Name = "Supplier Contact")]
            public System.Guid ContactId { get; set; }


            [Display(Name = "Amount (Line Items)")]
            //[DataType(DataType.Currency)]
            public Nullable<decimal> LineItemsAmount { get; set; }

            [Display(Name = "Tax (%)")]
            public Nullable<decimal> TaxPercent { get; set; }

            [Display(Name = "Post Tax Amount")]
            //[DataType(DataType.Currency)]
            public Nullable<decimal> PostTaxAmount { get; set; }

            [Display(Name = "Other Charges")]
            public Nullable<decimal> OtherCharges { get; set; }

            [Display(Name = "Supplier Address")]
            public Nullable<System.Guid> SupplierAddressId { get; set; }

            [Display(Name = "Name")]
            public string SupplierAddressToName { get; set; }
            
            [Display(Name = "Address Line 1")]
            public string SupplierAddressLine1 { get; set; }

            [Display(Name = "Address Line 2")]
            public string SupplierAddressLine2 { get; set; }

            [Display(Name = "Address Line 3")]
            public string SupplierAddressLine3 { get; set; }

            [Display(Name = "City")]
            public string SupplierAddressCity { get; set; }

            [Display(Name = "PIN")]
            public string SupplierAddressPin { get; set; }

            [Display(Name = "State")]
            public string SupplierAddressState { get; set; }

            [Display(Name = "Country")]
            public string SupplierAddressCountry { get; set; }

            [Display(Name = "Phone")]
            public string SupplierAddressPhone { get; set; }

            [Display(Name = "Email")]
            public string SupplierAddressEMail { get; set; }

            [Display(Name = "Billing Address")]
            public Nullable<System.Guid> BillingAddressId { get; set; }

            [Display(Name = "Bill To Name")]
            public string BillingAddressToName { get; set; }

            [Display(Name = "Address Line 1")]
            public string BillingAddressLine1 { get; set; }

            [Display(Name = "Address Line 2")]
            public string BillingAddressLine2 { get; set; }

            [Display(Name = "Address Line 3")]
            public string BillingAddressLine3 { get; set; }

            [Display(Name = "City")]
            public string BillingAddressCity { get; set; }

            [Display(Name = "PIN")]
            public string BillingAddressPin { get; set; }

            [Display(Name = "State")]
            public string BillingAddressState { get; set; }

            [Display(Name = "Country")]
            public string BillingAddressCountry { get; set; }

            [Display(Name = "Phone")]
            public string BillingAddressPhone { get; set; }

            [Display(Name = "Shipping Address")]
            public Nullable<System.Guid> ShippingAddressId { get; set; }

            [Display(Name = "Ship To Name")]
            public string ShippingAddressToName { get; set; }

            [Display(Name = "Address Line 1")]
            public string ShippingAddressLine1 { get; set; }

            [Display(Name = "Address Line 2")]
            public string ShippingAddressLine2 { get; set; }

            [Display(Name = "Address Line 3")]
            public string ShippingAddressLine3 { get; set; }

            [Display(Name = "City")]
            public string ShippingAddressCity { get; set; }

            [Display(Name = "PIN")]
            public string ShippingAddressPin { get; set; }

            [Display(Name = "State")]
            public string ShippingAddressState { get; set; }

            [Display(Name = "Country")]
            public string ShippingAddressCountry { get; set; }

            [Display(Name = "Phone")]
            public string ShippingAddressPhone { get; set; }

            [Display(Name = "Payment Terms")]
            public string PaymentTerms { get; set; }

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

        }
    }
}
