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
    
    public partial class Employee
    {
        public Employee()
        {
            this.JobIssueHeads = new HashSet<JobIssueHead>();
            this.JobReceiptHeads = new HashSet<JobReceiptHead>();
            this.StockIssueHeads = new HashSet<StockIssueHead>();
            this.StockReceiptHeads = new HashSet<StockReceiptHead>();
            this.ProcurementRequests = new HashSet<ProcurementRequest>();
            this.Attendances = new HashSet<Attendance>();
        }
    
        public System.Guid Id { get; set; }
        public string ShrtDesc { get; set; }
        public string Designation { get; set; }
        public System.Guid DepartmentId { get; set; }
        public System.Guid ManagerId { get; set; }
        public Nullable<decimal> Grade { get; set; }
        public System.DateTime DOJ { get; set; }
        public Nullable<System.DateTime> DOL { get; set; }
        public bool PFFlag { get; set; }
        public bool ESIFlag { get; set; }
        public bool FoodFlag { get; set; }
        public Nullable<decimal> FoodRate { get; set; }
        public Nullable<decimal> HourlyRate { get; set; }
        public bool Employed { get; set; }
        public bool HasLeft { get; set; }
        public Nullable<decimal> Basic { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> ExecA { get; set; }
        public Nullable<decimal> ConvA { get; set; }
        public Nullable<decimal> TeaA { get; set; }
        public Nullable<decimal> WashA { get; set; }
        public Nullable<decimal> CEA { get; set; }
        public Nullable<decimal> OthA { get; set; }
        public Nullable<decimal> PFFix { get; set; }
        public Nullable<decimal> CarLInst { get; set; }
        public Nullable<decimal> IncomeTAXInst { get; set; }
        public Nullable<decimal> ProfTAXInst { get; set; }
        public Nullable<decimal> EduTAXInst { get; set; }
        public Nullable<decimal> ServiceTAXInst { get; set; }
        public Nullable<decimal> OtherTAXInst { get; set; }
        public Nullable<decimal> LifeInsuInst { get; set; }
        public Nullable<decimal> HealthInsuInst { get; set; }
        public Nullable<decimal> LoanWInst { get; set; }
        public Nullable<decimal> LoanWOInst { get; set; }
        public string PFNo { get; set; }
        public string ESINo { get; set; }
        public string Bank { get; set; }
        public string BankAcNo { get; set; }
        public string LifeInsuNo { get; set; }
        public string HealthInsuNo { get; set; }
        public string GratuityNo { get; set; }
        public string PAN { get; set; }
        public string EmpVend { get; set; }
        public string DISPNo { get; set; }
        public Nullable<System.DateTime> LastPerformanceReviewDate { get; set; }
        public Nullable<System.DateTime> NextPerformanceReviewDate { get; set; }
        public Nullable<System.DateTime> LastCompensationReviewDate { get; set; }
        public Nullable<System.DateTime> NextCompensationReviewDate { get; set; }
        public string Remarks { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual Department Department { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<JobIssueHead> JobIssueHeads { get; set; }
        public virtual ICollection<JobReceiptHead> JobReceiptHeads { get; set; }
        public virtual ICollection<StockIssueHead> StockIssueHeads { get; set; }
        public virtual ICollection<StockReceiptHead> StockReceiptHeads { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual Contact Contact1 { get; set; }
        public virtual ICollection<ProcurementRequest> ProcurementRequests { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}
