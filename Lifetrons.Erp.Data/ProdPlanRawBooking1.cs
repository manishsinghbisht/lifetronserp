
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProdPlanRawBooking")]
    [MetadataType(typeof(ProdPlanRawBookingMetadata))]
    public partial class ProdPlanRawBooking : Entity
    {
        internal sealed class ProdPlanRawBookingMetadata
        {
            [Display(Name = "Job No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal JobNo { get; set; }

            [Required]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal Quantity { get; set; }

            [Display(Name = "Item")]
            public Nullable<System.Guid> ItemId { get; set; }

             [Display(Name = "Weight")]
            public Nullable<decimal> Weight { get; set; }
        }
    }
}
