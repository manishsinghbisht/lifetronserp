//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lifetrons.Erp.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contract
    {
        public Contract()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ShrtDesc { get; set; }
        public string Desc { get; set; }
        public string OwnerId { get; set; }
        public string ContractNo { get; set; }
        public string RefNo { get; set; }
        public Nullable<System.Guid> OpportunityId { get; set; }
        public Nullable<System.Guid> QuoteId { get; set; }
        public System.Guid AccountId { get; set; }
        public System.Guid PriceBookId { get; set; }
        public Nullable<System.Guid> CustSignedById { get; set; }
        public string CustSignedByTitle { get; set; }
        public Nullable<System.DateTime> CustSignedByDate { get; set; }
        public Nullable<System.DateTime> ContractStartDate { get; set; }
        public Nullable<decimal> ContractTenure { get; set; }
        public Nullable<System.DateTime> ContractExpirationDate { get; set; }
        public Nullable<decimal> ExpirationNotice { get; set; }
        public string CompanySignedById { get; set; }
        public Nullable<System.DateTime> CompanySignedDate { get; set; }
        public string DeliveryTerms { get; set; }
        public string PaymentTerms { get; set; }
        public string SpecialTerms { get; set; }
        public Nullable<System.Guid> BillingAddressId { get; set; }
        public Nullable<System.Guid> ShippingAddressId { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
        public string CustomColumn1 { get; set; }
        public Nullable<System.Guid> ColExtensionId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string BillingAddressToName { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingAddressLine3 { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressPin { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressCountry { get; set; }
        public string BillingAddressPhone { get; set; }
        public string BillingAddressEMail { get; set; }
        public string ShippingAddressToName { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingAddressLine3 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressPin { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressCountry { get; set; }
        public string ShippingAddressPhone { get; set; }
        public string ShippingAddressEMail { get; set; }
        public string SharedWith { get; set; }
    
        public virtual AspNetUser AspNetUserCmpSignedBy { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual Quote Quote { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Opportunity Opportunity { get; set; }
        public virtual PriceBook PriceBook { get; set; }
        public virtual Address Address { get; set; }
        public virtual Address Address1 { get; set; }
        public virtual Account Account { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
