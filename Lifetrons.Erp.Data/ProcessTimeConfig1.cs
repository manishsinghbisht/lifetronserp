
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProcessTimeConfig")]
    [MetadataType(typeof(ProcessTimeConfigMetadata))]
    public partial class ProcessTimeConfig : Entity
    {
        internal sealed class ProcessTimeConfigMetadata
        {
            [Display(Name = "Process")]
            [Required]
            public System.Guid ProcessId { get; set; }

            [Required]
            [DataType(DataType.Time)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
            [Display(Name = "Start Time")]
            public System.TimeSpan StartTime { get; set; }

            [Required]
            [DataType(DataType.Time)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
            [Display(Name = "End Time")]
            public System.TimeSpan EndTime { get; set; }

            [Required]
            [Display(Name = "From Date")]
            public System.DateTime FromDate { get; set; }

            [Required]
            [Display(Name = "To Date")]
            public System.DateTime ToDate { get; set; }
        }
    }
}
