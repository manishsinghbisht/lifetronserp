using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TaskStatu")]
    [MetadataType(typeof(TaskStatuMetadata))]
    public partial class TaskStatu : Entity
    {
        internal sealed class TaskStatuMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(4)]
            public string Code { get; set; }

        }
    }
}
