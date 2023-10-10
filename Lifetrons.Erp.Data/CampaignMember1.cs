using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using System;
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CampaignMember")]
    [MetadataType(typeof(CampaignMemberMetadata))]
    public partial class CampaignMember : Entity
    {
        internal sealed class CampaignMemberMetadata
        {

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CampaignId_Campaign")]
            public System.Guid CampaignId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_LeadId_Lead")]
            public Nullable<System.Guid> LeadId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ContactId_Contact")]
            public Nullable<System.Guid> ContactId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Status")]
            public System.Guid CampaignMemberStatusId { get; set; }
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
