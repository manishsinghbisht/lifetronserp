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
    
    public partial class Attendance
    {
        public System.Guid Id { get; set; }
        public System.Guid EmployeeId { get; set; }
        public System.DateTime InDateTime { get; set; }
        public System.DateTime OutDateTime { get; set; }
        public Nullable<int> WorkHours { get; set; }
        public string AtendanceStatus { get; set; }
        public string Remark { get; set; }
        public string CustomColumn1 { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Organization Organization { get; set; }
    }
}