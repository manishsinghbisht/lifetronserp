using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using System;
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Invoice")]
    [MetadataType(typeof(InvoiceMetadata))]
    public partial class Invoice : Entity
    {
        internal sealed class InvoiceMetadata
        {
            [Display(Name = "Invoice No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal InvoiceNo { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Code { get; set; }

            [Display(Name = "Short Description")]
            public string ShrtDesc { get; set; }

            [Required]
            [Display(Name = "Owner")]
            public string OwnerId { get; set; }

            [Display(Name = "Ref No")]
            public string RefNo { get; set; }

            [Display(Name = "Account")]
            public Nullable<System.Guid> AccountId { get; set; }

            [Display(Name = "Contact")]
            public Nullable<System.Guid> ContactId { get; set; }

            [Display(Name = "Opportunity")]
            public Nullable<System.Guid> OpportunityId { get; set; }

            [Display(Name = "Quote")]
            public Nullable<System.Guid> QuoteId { get; set; }

            [Display(Name = "Order")]
            public Nullable<System.Guid> OrderId { get; set; }

            [Display(Name = "Created By")]
            public string CreatedBy { get; set; }

            [Display(Name = "Created Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(Name = "Modified By")]
            public string ModifiedBy { get; set; }

            [Display(Name = "Modified Date")]
            public System.DateTime ModifiedDate { get; set; }


            [Display(Name = "Status")]
            public System.Guid InvoiceStatusId { get; set; }

            [Display(Name = "Invoice Date")]
            public System.DateTime InvoiceDate { get; set; }

            [Display(Name = "Payment Rcpt. Details")]
            public string PaymentReceiptDetails { get; set; }

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

            [Display(Name = "Invoice Amount")]
            //[DataType(DataType.Currency)]
            public Nullable<decimal> InvoiceAmount { get; set; }

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
        }
    }
}
