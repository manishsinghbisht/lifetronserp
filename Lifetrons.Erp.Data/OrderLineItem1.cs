
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrderLineItem")]
    [MetadataType(typeof(OrderLineItemMetadata))]
    public partial class OrderLineItem : Entity
    {
        internal sealed class OrderLineItemMetadata
        {

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_OrderId_Order")]
            public string OrderId { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_PriceBookId_PriceBook")]
            public string PriceBookId { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ProductId_Product")]
            public string ProductId { get; set; }

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


            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ShrtDesc_Description")]
            public string ShrtDesc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Desc_Updates")]
            public string Desc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Quantity")]
            [Required]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal Quantity { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_TotalWeight")]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> Weight { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_WeightUnit")]
            public Nullable<System.Guid> WeightUnitId { get; set; }

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

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Serial")]
            public Nullable<int> Serial { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_SpecialInstructions")]
            public string SpecialInstructions { get; set; }

            [Display(Name = "Prod. Quantity")]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal ProductionQuantity { get; set; }

            [Display(Name = "Prod. Weight")]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> ProductionWeight { get; set; }

            [Display(Name = "Prod. Weight Unit")]
            public Nullable<System.Guid> ProductionWeightUnitId { get; set; }

            [Display(Name = "Job No")]
            [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
            public decimal JobNo { get; set; }
        }
    }
}
