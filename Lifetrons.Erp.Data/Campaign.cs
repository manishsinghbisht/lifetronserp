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
    
    public partial class Campaign
    {
        public Campaign()
        {
            this.Campaign1 = new HashSet<Campaign>();
            this.Cases = new HashSet<Case>();
            this.Opportunities = new HashSet<Opportunity>();
            this.Leads = new HashSet<Lead>();
            this.CampaignMembers = new HashSet<CampaignMember>();
            this.Accounts = new HashSet<Account>();
            this.Contacts = new HashSet<Contact>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ShrtDesc { get; set; }
        public string Desc { get; set; }
        public string OwnerId { get; set; }
        public Nullable<decimal> NumberOfEmployees { get; set; }
        public string EmployeeDetails { get; set; }
        public System.Guid CampaignTypeId { get; set; }
        public System.Guid CampaignStatusId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<decimal> ExpectedRevenue { get; set; }
        public Nullable<decimal> BudgetCost { get; set; }
        public Nullable<decimal> ActualCost { get; set; }
        public Nullable<decimal> ExpectedResponsePercent { get; set; }
        public Nullable<decimal> NumSent { get; set; }
        public Nullable<System.Guid> ParentCampaignId { get; set; }
        public string Delivery { get; set; }
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
        public string SharedWith { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual CampaignType CampaignType { get; set; }
        public virtual CampaignStatu CampaignStatu { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Campaign> Campaign1 { get; set; }
        public virtual Campaign Campaign2 { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<Opportunity> Opportunities { get; set; }
        public virtual ICollection<Lead> Leads { get; set; }
        public virtual ICollection<CampaignMember> CampaignMembers { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
