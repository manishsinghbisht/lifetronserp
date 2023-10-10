

using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LeadStatu")]
    [MetadataType(typeof(LeadStatuMetadata))]
    public partial class LeadStatu : Entity
    {
        internal sealed class LeadStatuMetadata
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
