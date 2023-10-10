using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Item")]
    [MetadataType(typeof(ItemMetadata))]
    public partial class Item : Entity
    {
        internal sealed class ItemMetadata
        {
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Code")]
            [MaxLength(100)]
            public string Code { get; set; }

            [Display(Name = "Created By")]
            public string CreatedBy { get; set; }

            [Display(Name = "Created Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(Name = "Modified By")]
            public string ModifiedBy { get; set; }

            [Display(Name = "Modified Date")]
            public System.DateTime ModifiedDate { get; set; }
        }
    }
}
