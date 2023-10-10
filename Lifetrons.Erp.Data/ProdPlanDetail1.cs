
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProdPlanDetail")]
    [MetadataType(typeof(ProdPlanDetailMetadata))]
    public partial class ProdPlanDetail : Entity
    {
        internal sealed class ProdPlanDetailMetadata
        {
            [Display(Name = "Serial")]
            public Nullable<int> Serial { get; set; }

            [Required]
            [Display(Name = "Job No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal JobNo { get; set; }

            [Required]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal Quantity { get; set; }

            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            [Display(Name = "Weight")]
            public Nullable<decimal> Weight { get; set; }

            public Nullable<System.Guid> WeightUnitId { get; set; }

            [Display(Name = "Start Date Time")]
            [Required]
            public System.DateTime StartDateTime { get; set; }

            [Display(Name = "End Date Time")]
            [Required]
            public System.DateTime EndDateTime { get; set; }


            [Display(Name = "Actual Start Date Time")]
            public System.DateTime ActualStartDateTime { get; set; }

            [Display(Name = "Actual End Date Time")]
            public System.DateTime ActualEndDateTime { get; set; }

            [Display(Name = "Production Time (in hours)")]
            public decimal CycleTimeInHour { get; set; }

            [Display(Name = "Production Capacity")]
            public decimal CycleCapacity { get; set; }

            [Display(Name = "Production Qty per hour")]
            public Nullable<decimal> QuantityPerHour { get; set; }

            [Display(Name = "Setup time (in hours)")]
            public decimal SetupTimeInHrs { get; set; }

            [Display(Name = "Add On time (in hours)")]
            public decimal AddOnTimeInHrs { get; set; }
        }
    }
}
