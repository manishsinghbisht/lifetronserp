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
    
    public partial class Target
    {
        public System.Guid Id { get; set; }
        public string ObjectName { get; set; }
        public System.Guid ObjectId { get; set; }
        public System.DateTime TargetDate { get; set; }
        public decimal TargetFigure { get; set; }
        public string ShrtDesc { get; set; }
        public string Desc { get; set; }
        public string ClosingComments { get; set; }
        public string SharedWith { get; set; }
        public string OwnerId { get; set; }
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
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual Organization Organization { get; set; }
    }
}