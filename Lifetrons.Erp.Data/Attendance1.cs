

using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Attendance")]
    [MetadataType(typeof(AttendanceMetadata))]
    public partial class Attendance : Entity
    {
        internal sealed class AttendanceMetadata
        {

            [Display(Name = "Time In")]
            public string InDateTime { get; set; }

            [Display(Name = "Time Out")]
            public string OutDateTime { get; set; }


        }
    }
}
