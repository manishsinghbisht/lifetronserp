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
    
    public partial class Opportunity
    {
        public Opportunity()
        {
            this.Cases = new HashSet<Case>();
            this.Contracts = new HashSet<Contract>();
            this.Quotes = new HashSet<Quote>();
            this.Invoices = new HashSet<Invoice>();
            this.OpportunityLineItems = new HashSet<OpportunityLineItem>();
            this.Orders = new HashSet<Order>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ShrtDesc { get; set; }
        public string OwnerId { get; set; }
        public bool Private { get; set; }
        public Nullable<System.Guid> CampaignId { get; set; }
        public Nullable<System.Guid> LeadId { get; set; }
        public Nullable<System.Guid> ContactId { get; set; }
        public Nullable<System.Guid> AccountId { get; set; }
        public System.Guid OpportunityTypeId { get; set; }
        public System.Guid LeadSourceId { get; set; }
        public string RefNo { get; set; }
        public string OrderNo { get; set; }
        public Nullable<decimal> NumberOfEmployees { get; set; }
        public Nullable<decimal> ExpectedRevenue { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
        public string NextStep { get; set; }
        public System.Guid StageId { get; set; }
        public Nullable<decimal> ProbabilityPercent { get; set; }
        public Nullable<System.Guid> DeliveryStatusId { get; set; }
        public string Competitors { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
        public string CustomColumn1 { get; set; }
        public string CustomColumn2 { get; set; }
        public Nullable<System.Guid> ColExtensionId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string SharedWith { get; set; }
        public Nullable<decimal> LineItemsAmount { get; set; }
        public Nullable<decimal> LineItemsQuantity { get; set; }
        public string Remark { get; set; }
        public decimal OpportunityNo { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual DeliveryStatu DeliveryStatu { get; set; }
        public virtual LeadSource LeadSource { get; set; }
        public virtual OpportunityType OpportunityType { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual ICollection<Quote> Quotes { get; set; }
        public virtual Campaign Campaign { get; set; }
        public virtual Lead Lead { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<OpportunityLineItem> OpportunityLineItems { get; set; }
        public virtual Account Account { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}