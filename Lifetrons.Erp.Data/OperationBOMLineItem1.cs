using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperationBOMLineItem")]
    [MetadataType(typeof(OperationBOMLineItemMetadata))]
    public partial class OperationBOMLineItem : Entity
    {
        internal sealed class OperationBOMLineItemMetadata
        {

            [Display(Name = "Product")]
            [Key, Column(Order = 0)]
            public System.Guid ProductId { get; set; }

            [Display(Name = "EStage")]
            [Key, Column(Order = 1)]
            public System.Guid EnterpriseStageId { get; set; }

            [Display(Name = "Process")]
            [Key, Column(Order = 2)]
            public System.Guid ProcessId { get; set; }

            [Display(Name = "Item")]
            [Key, Column(Order = 3)]
            public System.Guid ItemId { get; set; }
        }
    }
}
