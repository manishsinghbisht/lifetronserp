
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DispatchLineItem")]
    [MetadataType(typeof(DispatchLineItemMetadata))]
    public partial class DispatchLineItem : Entity
    {
        internal sealed class DispatchLineItemMetadata
        {
            [Required]
            public decimal Quantity { get; set; }

            [Display(Name = "Product")]
            public Nullable<System.Guid> ProductId { get; set; }

             [Display(Name = "Weight")]
            public Nullable<decimal> Weight { get; set; }
        }
    }
}
