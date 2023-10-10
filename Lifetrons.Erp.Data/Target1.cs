using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Target")]
    [MetadataType(typeof(TargetMetadata))]
    public partial class Target : Entity
    {
        internal sealed class TargetMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Target Type")]
            public string ObjectName { get; set; }

            [Required]
            [Display(Name = "Target For")]
            public System.Guid ObjectId { get; set; }

            [Required]
            [Display(Name = "Target Date")]
            public System.DateTime TargetDate { get; set; }

            [Required]
            [Display(Name = "Target Figure")]
            public decimal TargetFigure { get; set; }
           
            [Display(Name = "Short Desc")]
            public string ShrtDesc { get; set; }

            [Display(Name = "Updates")]
            public string Desc { get; set; }

            [Display(Name = "Owner")]
            public string OwnerId { get; set; }

            [Display(Name = "Closing Comments")]
            public string ClosingComments { get; set; }

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
