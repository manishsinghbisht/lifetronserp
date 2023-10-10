using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using System;
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NoticeBoard")]
    [MetadataType(typeof(NoticeBoardMetadata))]
    public partial class NoticeBoard : Entity
    {
        internal sealed class NoticeBoardMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "NoticeBoardMetadata_Name_Subject")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_Code_Code")]
            public string Code { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "NoticeBoardMetadata_OpeningDate_Opening_Date")]
            public System.DateTime OpeningDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "NoticeBoardMetadata_CloseDate_Close_Date")]
            public Nullable<System.DateTime> CloseDate { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "NoticeBoardMetadata_ClosingComments_Close_Comments")]
            public string ClosingComments { get; set; }

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
