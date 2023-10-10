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
    
    public partial class Media
    {
        public System.Guid Id { get; set; }
        public string ParentType { get; set; }
        public System.Guid ParentId { get; set; }
        public string MediaType { get; set; }
        public string MediaPath { get; set; }
        public string MediaName { get; set; }
        public string Tags { get; set; }
        public string ShrtDesc { get; set; }
        public string Desc { get; set; }
        public bool IsMarkedAbuse { get; set; }
        public string MarkedAbuseByUserId { get; set; }
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
    
        public virtual AspNetUser AspNetUserCreatedBy { get; set; }
        public virtual AspNetUser AspNetUserMarkedAbusedBy { get; set; }
        public virtual AspNetUser AspNetUserModifiedBy { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
