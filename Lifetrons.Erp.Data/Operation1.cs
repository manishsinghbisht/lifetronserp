
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Operation")]
    [MetadataType(typeof(OperationMetadata))]
    public partial class Operation : Entity
    {
        internal sealed class OperationMetadata
        {
            [Required]
            public Nullable<int> Serial { get; set; }

            [Display(Name = "Product")]
            [Key, Column(Order = 0)]
            public System.Guid ProductId { get; set; }

            [Display(Name = "EStage")]
            [Key, Column(Order = 1)]
            public System.Guid EnterpriseStageId { get; set; }

            [Display(Name = "Process")]
            [Key, Column(Order = 2)]
            public System.Guid ProcessId { get; set; }

            [Display(Name = "Labour Rate / hour")]
            public Nullable<decimal> LabourRatePerHour { get; set; }

            [Display(Name = "Depreciation Rate / hour")]
            public Nullable<decimal> DepreciationRatePerHour { get; set; }

            [Display(Name = "Energy Rate / hour")]
            public Nullable<decimal> EnergyRatePerHour { get; set; }

            [Display(Name = "Overhead Rate / hour")]
            public Nullable<decimal> OverheadRatePerHour { get; set; }

            [Display(Name = "Other Direct Expenses Rate / hour")]
            public Nullable<decimal> OtherDirectExpensesRatePerHour { get; set; }

            [Display(Name = "Other InDirect Expenses Rate / hour")]
            public Nullable<decimal> OtherInDirectExpensesRatePerHour { get; set; }

            [Display(Name = "Rate / hour")]
            public Nullable<decimal> RatePerHour { get; set; }

            [Display(Name = "Cycle time (hrs)")]
            public decimal CycleTimeInHour { get; set; }

            [Display(Name = "Capacity per cycle")]
            public decimal CycleCapacity { get; set; }

            [Display(Name = "Quantity per hour")]
            public decimal QuantityPerHour { get; set; }

            [Display(Name = "Per unit cost")]
            public Nullable<decimal> PerUnitCost { get; set; }

            [Display(Name = "Remark")]
            public string Remark { get; set; }

            [Display(Name = "Type")]
            public string Type { get; set; }
        }
    }
}