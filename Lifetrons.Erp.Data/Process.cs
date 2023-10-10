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
    
    public partial class Process
    {
        public Process()
        {
            this.JobIssueHeads = new HashSet<JobIssueHead>();
            this.JobIssueHeads1 = new HashSet<JobIssueHead>();
            this.JobReceiptHeads = new HashSet<JobReceiptHead>();
            this.JobReceiptHeads1 = new HashSet<JobReceiptHead>();
            this.Operations = new HashSet<Operation>();
            this.StockIssueHeads = new HashSet<StockIssueHead>();
            this.StockIssueHeads1 = new HashSet<StockIssueHead>();
            this.StockReceiptHeads = new HashSet<StockReceiptHead>();
            this.StockReceiptHeads1 = new HashSet<StockReceiptHead>();
            this.ProdPlanDetails = new HashSet<ProdPlanDetail>();
            this.ProcessTimeConfigs = new HashSet<ProcessTimeConfig>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ShrtDesc { get; set; }
        public System.Guid EnterpriseStageId { get; set; }
        public Nullable<decimal> LabourRatePerHour { get; set; }
        public Nullable<decimal> DepreciationRatePerHour { get; set; }
        public Nullable<decimal> EnergyRatePerHour { get; set; }
        public Nullable<decimal> OverheadRatePerHour { get; set; }
        public Nullable<decimal> OtherDirectExpensesRatePerHour { get; set; }
        public Nullable<decimal> OtherInDirectExpensesRatePerHour { get; set; }
        public Nullable<decimal> RatePerHour { get; set; }
        public decimal CycleTimeInHour { get; set; }
        public string Remark { get; set; }
        public System.Guid OrgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
        public Nullable<int> Serial { get; set; }
        public decimal CycleCapacity { get; set; }
        public Nullable<decimal> QuantityPerHour { get; set; }
        public Nullable<decimal> PerUnitCost { get; set; }
        public string Type { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual EnterpriseStage EnterpriseStage { get; set; }
        public virtual ICollection<JobIssueHead> JobIssueHeads { get; set; }
        public virtual ICollection<JobIssueHead> JobIssueHeads1 { get; set; }
        public virtual ICollection<JobReceiptHead> JobReceiptHeads { get; set; }
        public virtual ICollection<JobReceiptHead> JobReceiptHeads1 { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<StockIssueHead> StockIssueHeads { get; set; }
        public virtual ICollection<StockIssueHead> StockIssueHeads1 { get; set; }
        public virtual ICollection<StockReceiptHead> StockReceiptHeads { get; set; }
        public virtual ICollection<StockReceiptHead> StockReceiptHeads1 { get; set; }
        public virtual ICollection<ProdPlanDetail> ProdPlanDetails { get; set; }
        public virtual ICollection<ProcessTimeConfig> ProcessTimeConfigs { get; set; }
    }
}