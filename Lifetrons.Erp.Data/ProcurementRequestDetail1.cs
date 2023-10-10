
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProcurementRequestDetail")]
    [MetadataType(typeof(ProcurementRequestDetailMetadata))]
    public partial class ProcurementRequestDetail : Entity
    {
        internal sealed class ProcurementRequestDetailMetadata
        {
            [Display(Name = "Raw Booking")]
            public Nullable<System.Guid> ProdPlanRawBookingId { get; set; }
            public Nullable<int> Serial { get; set; }

            [Display(Name = "Required by date")]
            public Nullable<System.DateTime> RequiredByDate { get; set; }

            [Display(Name = "Item")]
            [Required]
            public Nullable<System.Guid> ItemId { get; set; }

            [Required]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal Quantity { get; set; }

            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            [Display(Name = "Weight")]
            public Nullable<decimal> Weight { get; set; }

            [Display(Name = "Estimated unit cost")]
            public Nullable<decimal> EstimatedCost { get; set; }

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
