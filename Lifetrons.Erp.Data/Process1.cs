
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Process")]
    [MetadataType(typeof(ProcessMetadata))]
    public partial class Process : Entity
    {
        internal sealed class ProcessMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(100)]
            public string Code { get; set; }

            [Display(Name = "EStage")]
            public System.Guid EnterpriseStageId { get; set; }

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