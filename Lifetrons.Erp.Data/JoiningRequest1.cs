using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    [Table("JoiningRequest")]
    [MetadataType(typeof(JoiningRequestMetadata))]
    public partial class JoiningRequest:Entity
    {
        internal sealed class JoiningRequestMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "JoiningRequestMetadata_RequestComments_Request_Comments")]
            public string RequestComments { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "JoiningRequestMetadata_SubmittedTo_Submit_To_Username")]
            public string SubmittedTo { get; set; }

            [Required]
            [EnumDataType(typeof(StatusValues))]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Status")]
            public string Status { get; set; }
            public enum StatusValues
            {
                PENDING, APPROVED, REJECTED
            }
        }

    }
}
