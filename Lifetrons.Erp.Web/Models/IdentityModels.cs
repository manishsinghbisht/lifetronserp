using System.ComponentModel;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lifetrons.Erp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        
        //Enable-Migrations
        //Add-Migration Init
        //Update-Database

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_AuthenticatedEmail_Invalid_Authenticated_Email_Address")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_AuthenticatedEmail_Authenticated_Email")]
        [MaxLength(400)]
        public string AuthenticatedEmail { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_Email_Invalid_Email_Address")]
        [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessage = "Invalid mail id.")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_Email_Work_Email")]
        [MaxLength(400)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_FirstName_Invalid_first_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_FirstName_First_Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_LastName_Invalid_last_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_LastName_Last_Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_BirthDate_Invalid_birth_date")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_BirthDate_Birth_Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? BirthDate { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_Mobile_Invalid_mobile_number")]
        //[RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_Mobile_Invalid_mobile_number_format_")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_Mobile_Mobile__10_digit_")]
        public string Mobile { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_AddressLine1_Invalid_address_line_1")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_AddressLine1_Address_Line_1")]
        public string AddressLine1 { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_AddressLine2_Invalid_address_line_2")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_AddressLine2_Address_Line_2")]
        public string AddressLine2 { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_City_Invalid_city_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_City_City")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_State_Invalid_state_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_State_State")]
        public string State { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_Country_Invalid_country_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_Country_Country")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.PostalCode, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ApplicationUser_PostalCode_Invalid_postal_code")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ApplicationUser_PostalCode_Postal_Code")]
        public string PostalCode { get; set; }

        public string Culture { get; set; }

        public string TimeZone { get; set; }

        public Guid? OrgId { get; set; }

        [DefaultValue(false)]
        public bool Active { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(string.IsNullOrEmpty(Helper.ConnectionStringName) ? "DefaultConnection" : Helper.ConnectionStringName , throwIfV1Schema: false)
        {
        }
    }
}