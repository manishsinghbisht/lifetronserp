

using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Ownership")]
    [MetadataType(typeof(OwnershipMetadata))]
    public partial class Ownership : Entity
    {
        internal sealed class OwnershipMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(4)]
            [Display(Name = "Code")]
            public string Code { get; set; }
        }
    }
}
