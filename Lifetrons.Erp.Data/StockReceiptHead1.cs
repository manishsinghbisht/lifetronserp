
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StockReceiptHead")]
    [MetadataType(typeof(StockReceiptHeadMetadata))]
    public partial class StockReceiptHead : Entity
    {
        internal sealed class StockReceiptHeadMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Receipt By EStage")]
            public System.Guid ReceiptByEnterpriseStageId { get; set; }

            [Display(Name = "Receipt By Process")]
            public System.Guid ReceiptByProcessId { get; set; }

            [Display(Name = "Receipt From EStage")]
            public System.Guid ReceiptFromEnterpriseStageId { get; set; }

            [Display(Name = "Receipt From Process")]
            public System.Guid ReceiptFromProcessId { get; set; }

            [Display(Name = "Employee")]
            public Nullable<System.Guid> EmployeeId { get; set; }
        }
    }
}
