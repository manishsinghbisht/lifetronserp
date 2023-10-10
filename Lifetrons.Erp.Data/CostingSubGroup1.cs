using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CostingSubGroup")]
    [MetadataType(typeof(CostingSubGroupMetadata))]
    public partial class CostingSubGroup : Entity
    {
        internal sealed class CostingSubGroupMetadata
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
