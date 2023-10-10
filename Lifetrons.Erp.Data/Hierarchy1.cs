
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Hierarchy")]
    [MetadataType(typeof(HierarchyMetadata))]
    public partial class Hierarchy : Entity
    {
        internal sealed class HierarchyMetadata
        {
            [Display(Name = "Username")]
            public string UserId { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_DepartmentId_Department")]
            public System.Guid DepartmentId { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_TeamId_Team")]
            public System.Guid TeamId { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "HierarchyMetadata_ReportsTo_Reports_To")]
            public string ReportsTo { get; set; }

            [MaxLength(100)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "HierarchyMetadata_Designation_Designation")]
            public string Designation { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_Organization")]
            public System.Guid OrgId { get; set; }
            
        }
    }
}
