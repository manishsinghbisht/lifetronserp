
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StockItemIssue")]
    [MetadataType(typeof(StockItemIssueMetadata))]
    public partial class StockItemIssue : Entity
    {
        internal sealed class StockItemIssueMetadata
        {
            [Display(Name = "Job No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal JobNo { get; set; }

            [Display(Name = "Case No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal CaseNo { get; set; }

            [Display(Name = "Task No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal TaskNo { get; set; }

            [Required]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal Quantity { get; set; }

            [Display(Name = "Item")]
            public System.Guid ItemId { get; set; }

           
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            [Display(Name = "Weight")]
            public Nullable<decimal> Weight { get; set; }
        }
    }
}
