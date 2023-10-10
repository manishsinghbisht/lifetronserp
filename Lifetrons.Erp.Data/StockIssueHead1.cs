
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StockIssueHead")]
    [MetadataType(typeof(StockIssueHeadMetadata))]
    public partial class StockIssueHead : Entity
    {
        internal sealed class StockIssueHeadMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Date")]
            public System.DateTime Date { get; set; }

            [Display(Name = "Issued By EStage")]
            public System.Guid IssuedByEnterpriseStageId { get; set; }

            [Display(Name = "Issued By Process")]
            public System.Guid IssuedByProcessId { get; set; }

            [Display(Name = "Issued To EStage")]
            public System.Guid IssuedToEnterpriseStageId { get; set; }

            [Display(Name = "Issued To Process")]
            public System.Guid IssuedToProcessId { get; set; }

            [Display(Name = "Employee")]
            public Nullable<System.Guid> EmployeeId { get; set; }
        }
    }
}
