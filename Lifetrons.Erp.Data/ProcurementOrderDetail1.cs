
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProcurementOrderDetail")]
    [MetadataType(typeof(ProcurementOrderDetailMetadata))]
    public partial class ProcurementOrderDetail : Entity
    {
        internal sealed class ProcurementOrderDetailMetadata
        {
            [Display(Name = "Requisition")]
            public System.Guid ProcurementRequestDetailId { get; set; }

            public Nullable<int> Serial { get; set; }

        
            [Display(Name = "Item")]
            [Required]
            public Nullable<System.Guid> ItemId { get; set; }

            [Required]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal Quantity { get; set; }

            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            [Display(Name = "Weight")]
            public Nullable<decimal> Weight { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_DiscountPercent_Per_Item_Discount")]
            public Nullable<decimal> DiscountPercent { get; set; }

           [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_DiscountAmount_Per_Item_Discount")]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> DiscountAmount { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_SalesPrice")]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal SalesPrice { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_LineItemPrice")]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> LineItemPrice { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_LineItemAmount")]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> LineItemAmount { get; set; }

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
