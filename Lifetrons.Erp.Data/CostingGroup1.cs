using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CostingGroup")]
    [MetadataType(typeof(CostingGroupMetadata))]
    public partial class CostingGroup : Entity
    {
        internal sealed class CostingGroupMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Code { get; set; }
            
        }
    }
}
