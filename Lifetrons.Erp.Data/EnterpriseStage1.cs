using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EnterpriseStage")]
    [MetadataType(typeof(EnterpriseStageMetadata))]
    public partial class EnterpriseStage : Entity
    {
        internal sealed class EnterpriseStageMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Code { get; set; }

            [Display(Name = "Department")]
            public Nullable<System.Guid> DepartmentId { get; set; }

            [Display(Name = "Type")]
            public string Type { get; set; }
        }
    }
}