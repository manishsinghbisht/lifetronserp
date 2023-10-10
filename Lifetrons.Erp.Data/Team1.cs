

using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Team")]
    [MetadataType(typeof(TeamMetadata))]
    public partial class Team : Entity
    {
        internal sealed class TeamMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(390)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "TeamMetadata_Name_Team_Name")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(390)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Code_Code")]
            public string Code { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ShrtDesc_Description")]
            public string ShrtDesc { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "TeamMetadata_DepartmentId_Parent_Department")]
            public System.Guid DepartmentId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Organization")]
            public System.Guid OrgId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Authorized")]
            public bool Authorized { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Active")]
            public bool Active { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            public System.DateTime ModifiedDate { get; set; }
        }
    }
}
