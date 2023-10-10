

using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Product")]
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product: Entity
    {

        internal sealed class ProductMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Name_Name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Code_Code")]
            public string Code { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ShrtDesc_Description")]
            public string ShrtDesc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Desc_Updates")]
            public string Desc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            [ExcludeFromEqualityComparison("ModifiedBy")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            [ExcludeFromEqualityComparison("ModifiedDate")]
            public System.DateTime ModifiedDate { get; set; }

            [ExcludeFromEqualityComparison("OrgId")]
            public System.Guid OrgId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ProductMetadata_ProductFamilyId_Product_Family")]
            public System.Guid ProductFamilyId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "ProductMetadata_ProductTypeId_Product_Type")]
            public System.Guid ProductTypeId { get; set; }


            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Authorized")]
            public bool Authorized { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Active")]
            public bool Active { get; set; }
        }
    }
}