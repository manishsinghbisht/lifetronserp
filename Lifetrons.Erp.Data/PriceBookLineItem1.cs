
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PriceBookLineItem")]
    [MetadataType(typeof(PriceBookLineItemMetadata))]
    public partial class PriceBookLineItem: Entity
    {
        internal sealed class PriceBookLineItemMetadata
        {
            
            [Required]
            [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public decimal ListPrice { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_PriceBookId_PriceBook")]
            public System.Guid PriceBookId { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ProductId_Product")]
            public System.Guid ProductId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            public System.DateTime ModifiedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Authorized")]
            public bool Authorized { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Active")]
            public bool Active { get; set; }
        }
    }
}
