

using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("Organization")]
    [MetadataType(typeof(OrganizationMetadata))]
    public partial class Organization : Entity
    {
        internal sealed class OrganizationMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            public string Code { get; set; }

            [Required]
            [DataType(DataType.EmailAddress, ErrorMessage = "Invalid mail")]
            [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessage = "Invalid mail id.")]
            [MaxLength(400)]
            public string Email1 { get; set; }

            [Required]
            [DataType(DataType.EmailAddress, ErrorMessage = "Invalid mail")]
            [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessage = "Invalid mail id.")]
            [MaxLength(400)]
            public string Email2 { get; set; }

            [Required]
            //[RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ApplicationUser_Mobile_Invalid_mobile_number_format_")]
            public string Phone1 { get; set; }


            //[RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ApplicationUser_Mobile_Invalid_mobile_number_format_")]
            public string Phone2 { get; set; }

            [Required]
            [DataType(DataType.Text)]
            public string AddressLine1 { get; set; }

            //[Required]
            [DataType(DataType.Text)]
            public string AddressLine2 { get; set; }

            [Required]
            [DataType(DataType.Text)]
            public string City { get; set; }

            [Required]
            [DataType(DataType.Text)]
            public string State { get; set; }

            [Required]
            [DataType(DataType.Text)]
            public string Country { get; set; }

            [Required]
            [DataType(DataType.PostalCode)]
            public string PostalCode { get; set; }

            [Required]
            [DataType(DataType.EmailAddress, ErrorMessage = "Invalid approver mail")]
            [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessage = "Invalid approver mail id.")]
            [MaxLength(400)]
            public string ApproverEMail { get; set; }

            //[Required]
            //[RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ApplicationUser_Mobile_Invalid_mobile_number_format_")]
            public string ApproverPhone { get; set; }

        }
    }
}