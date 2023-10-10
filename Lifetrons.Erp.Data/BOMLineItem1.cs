using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BOMLineItem")]
    [MetadataType(typeof(BOMLineItemMetadata))]
    public partial class BOMLineItem : Entity
    {
        internal sealed class BOMLineItemMetadata
        {

            [Display(Name = "Product")]
            public System.Guid ProductId { get; set; }

            [Display(Name = "Item")]
            public System.Guid ItemId { get; set; }

        }
    }
}
