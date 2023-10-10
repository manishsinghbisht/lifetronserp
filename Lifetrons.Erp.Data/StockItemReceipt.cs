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
    
    public partial class StockItemReceipt
    {
        public System.Guid Id { get; set; }
        public System.Guid StockReceiptId { get; set; }
        public Nullable<int> Serial { get; set; }
        public System.Guid ItemId { get; set; }
        public Nullable<decimal> JobNo { get; set; }
        public decimal Quantity { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<System.Guid> WeightUnitId { get; set; }
        public Nullable<decimal> ExtraQuantity { get; set; }
        public Nullable<decimal> Expenses { get; set; }
        public string Remark { get; set; }
        public string CustomColumn1 { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
        public byte[] TimeStamp { get; set; }
        public Nullable<decimal> CaseNo { get; set; }
        public Nullable<decimal> TaskNo { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual Item Item { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual StockReceiptHead StockReceiptHead { get; set; }
        public virtual WeightUnit WeightUnit { get; set; }
    }
}
