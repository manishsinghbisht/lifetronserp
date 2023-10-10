using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BOM")]
    [MetadataType(typeof(BOMMetadata))]
    public partial class BOM : Entity
    {
        internal sealed class BOMMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(100)]
            public string Code { get; set; }
            
        }
    }
}
