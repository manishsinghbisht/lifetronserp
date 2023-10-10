
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PriceBook")]
    [MetadataType(typeof(PriceBookMetadata))]
    public partial class PriceBook:Entity
    {
        internal sealed class PriceBookMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Name_Name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Code_Code")]
            public string Code { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ShrtDesc_Description")]
            public string ShrtDesc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Desc_Updates")]
            public string Desc { get; set; }

            [DataType(DataType.Text)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_SharedWith")]
            public string SharedWith { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            public System.DateTime ModifiedDate { get; set; }


        }
    }
}
