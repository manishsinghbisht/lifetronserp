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
    
    public partial class Address
    {
        public Address()
        {
            this.Contracts = new HashSet<Contract>();
            this.Contracts1 = new HashSet<Contract>();
            this.Leads = new HashSet<Lead>();
            this.Quotes = new HashSet<Quote>();
            this.Quotes1 = new HashSet<Quote>();
            this.Invoices = new HashSet<Invoice>();
            this.Invoices1 = new HashSet<Invoice>();
            this.Accounts = new HashSet<Account>();
            this.Accounts1 = new HashSet<Account>();
            this.Contacts = new HashSet<Contact>();
            this.Contacts1 = new HashSet<Contact>();
            this.ProcurementOrders = new HashSet<ProcurementOrder>();
            this.ProcurementOrders1 = new HashSet<ProcurementOrder>();
            this.ProcurementOrders2 = new HashSet<ProcurementOrder>();
            this.Orders = new HashSet<Order>();
            this.Orders1 = new HashSet<Order>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ShrtDesc { get; set; }
        public string AddressType { get; set; }
        public string AddressToName { get; set; }
        public string AddressToTitle { get; set; }
        public string AddressToEMail { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Milestone { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string ServiceURI { get; set; }
        public string EMail { get; set; }
        public string Mobile { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string FAX { get; set; }
        public string FallBack { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<System.Guid> ColExtensionId { get; set; }
        public bool Active { get; set; }
        public bool Authorized { get; set; }
        public string SharedWith { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Contract> Contracts1 { get; set; }
        public virtual ICollection<Lead> Leads { get; set; }
        public virtual ICollection<Quote> Quotes { get; set; }
        public virtual ICollection<Quote> Quotes1 { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Invoice> Invoices1 { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Account> Accounts1 { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Contact> Contacts1 { get; set; }
        public virtual ICollection<ProcurementOrder> ProcurementOrders { get; set; }
        public virtual ICollection<ProcurementOrder> ProcurementOrders1 { get; set; }
        public virtual ICollection<ProcurementOrder> ProcurementOrders2 { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Order> Orders1 { get; set; }
    }
}