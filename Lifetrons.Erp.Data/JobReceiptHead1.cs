
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobReceiptHead")]
    [MetadataType(typeof(JobReceiptHeadMetadata))]
    public partial class JobReceiptHead : Entity
    {
        internal sealed class JobReceiptHeadMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Receipt From Process")]
            public System.Guid ReceiptFromProcessId { get; set; }

            [Display(Name = "Receipt By Process")]
            public System.Guid ReceiptByProcessId { get; set; }

            [Display(Name = "Employee")]
            public Nullable<System.Guid> EmployeeId { get; set; }
        }
    }
}
