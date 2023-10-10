
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Dispatch")]
    [MetadataType(typeof(DispatchMetadata))]
    public partial class Dispatch : Entity
    {
        internal sealed class DispatchMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Code { get; set; }

            [Display(Name = "Date")]
            public System.DateTime Date { get; set; }

            
        }
    }
}
