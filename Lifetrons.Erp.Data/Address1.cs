
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Address")]
    [MetadataType(typeof(AddressMetadata))]
    public partial class Address : Entity
    {
        internal sealed class AddressMetadata
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

            [Display(ResourceType = typeof (Resources.Resources), Name = "AddressMetadata_AddressType_Address_Type")]
            public string AddressType { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressToName")]
            public string AddressToName { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "AddressMetadata_AddressToTitle_Person_Designation")]
            public string AddressToTitle { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine1")]
            public string AddressLine1 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine2")]
            public string AddressLine2 { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressLine3")]
            public string AddressLine3 { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressMilestone")]
            public string Milestone { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCity")]
            public string City { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressState")]
            public string State { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressPin")]
            public string PostalCode { get; set; }

            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_AddressCountry")]
            public string Country { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_EMail")]
            public string AddressToEMail { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "Metadata_AddressFallBackContact")]
            public string FallBack { get; set; }

        }
    }
}
