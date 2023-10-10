
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobProductReceipt")]
    [MetadataType(typeof(JobProductReceiptMetadata))]
    public partial class JobProductReceipt : Entity
    {
        internal sealed class JobProductReceiptMetadata
        {
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

        }
    }
}
