
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobIssueHead")]
    [MetadataType(typeof(JobIssueHeadMetadata))]
    public partial class JobIssueHead : Entity
    {
        internal sealed class JobIssueHeadMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Issued By Process")]
            public System.Guid IssuedByProcessId { get; set; }

            [Display(Name = "Issued To Process")]
            public System.Guid IssuedToProcessId { get; set; }

            [Display(Name = "Employee")]
            public Nullable<System.Guid> EmployeeId { get; set; }
        }
    }
}
