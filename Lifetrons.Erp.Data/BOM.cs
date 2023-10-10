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
    
    public partial class BOM
    {
        public BOM()
        {
            this.BOMLineItems = new HashSet<BOMLineItem>();
        }
    
        public System.Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ShrtDesc { get; set; }
        public string Desc { get; set; }
        public decimal Quantity { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<System.Guid> WeightUnitId { get; set; }
        public Nullable<decimal> OtherCharges { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string OwnerId { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
        public byte[] TimeStamp { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Product Product { get; set; }
        public virtual WeightUnit WeightUnit { get; set; }
        public virtual ICollection<BOMLineItem> BOMLineItems { get; set; }
    }
}