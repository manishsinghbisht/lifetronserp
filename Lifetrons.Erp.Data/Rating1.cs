

using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Rating")]
    [MetadataType(typeof(RatingMetadata))]
    public partial class Rating : Entity
    {
        internal sealed class RatingMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(4)]
            public string Code { get; set; }
            
        }
    }
}
