using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

namespace Lifetrons.Erp.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        //Enable-Migrations
        //Add-Migration Init
        //Update-Database

        [Required]
        [Display(ResourceType = typeof (Resources.Resources), Name = "LoginViewModel_UserName_User_name")]
        [MaxLength(400)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "RegisterViewModel_AuthenticatedEmail_Invalid_Authenticated_Email_Address")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "RegisterViewModel_AuthenticatedEmail_Authenticated_Email")]
        [MaxLength(400)]
        public string AuthenticatedEmail { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_Email_Invalid_Email_Address")]
        [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessage = "Invalid mail id.")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_Email_Work_Email")]
        [MaxLength(400)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_FirstName_Invalid_first_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_FirstName_First_Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_LastName_Invalid_last_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_LastName_Last_Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_BirthDate_Invalid_birth_date")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_BirthDate_Birth_Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? BirthDate { get; set; }

        //((\+*)((0[ -]+)*|(91 )*)(\d{12}+|\d{10}+))|\d{5}([- ]*)\d{6}
        //^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$
        //^\\D?(\\d{3})\\D?\\D?(\\d{3})\\D?(\\d{4})$
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid mobile number")]
        //[RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid mobile number format.")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_AddressLine1_Invalid_address_line_1")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_AddressLine1_Address_Line_1")]
        public string AddressLine1 { get; set; }

        //[Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_AddressLine2_Invalid_address_line_2")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_AddressLine2_Address_Line_2")]
        public string AddressLine2 { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_City_Invalid_city_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_City_City")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_State_Invalid_state_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_State_State")]
        public string State { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_Country_Invalid_country_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_Country_Country")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.PostalCode, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_PostalCode_Invalid_postal_code")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_PostalCode_Postal_Code")]
        public string PostalCode { get; set; }

        public string Culture { get; set; }

        public string TimeZone { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ManageUserViewModel_OldPassword_Current_password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ManageUserViewModel_NewPassword_New_password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ManageUserViewModel_ConfirmPassword_Confirm_new_password")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ManageUserViewModel_ConfirmPassword_The_new_password_and_confirmation_password_do_not_match_")]
        public string ConfirmPassword { get; set; }
    }

    public class ManageUserPasswordViewModel
    {
        public string UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "ManageUserViewModel_NewPassword_New_password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "ManageUserViewModel_ConfirmPassword_Confirm_new_password")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ManageUserViewModel_ConfirmPassword_The_new_password_and_confirmation_password_do_not_match_")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(ResourceType = typeof (Resources.Resources), Name = "LoginViewModel_UserName_User_name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources.Resources), Name = "LoginViewModel_Password_Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof (Resources.Resources), Name = "LoginViewModel_RememberMe_Remember_me_")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(ResourceType = typeof (Resources.Resources), Name = "LoginViewModel_UserName_User_name")]
        [MaxLength(400)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources.Resources), Name = "LoginViewModel_Password_Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources.Resources), Name = "RegisterViewModel_ConfirmPassword_Confirm_password")]
        [Compare("Password", ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "RegisterViewModel_ConfirmPassword_The_password_and_confirmation_password_do_not_match_")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "RegisterViewModel_AuthenticatedEmail_Invalid_Authenticated_Email_Address")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "RegisterViewModel_AuthenticatedEmail_Authenticated_Email")]
        [MaxLength(400)]
        public string AuthenticatedEmail { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_Email_Invalid_Email_Address")]
        [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessage = "Invalid mail id.")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_Email_Work_Email")]
        [MaxLength(400)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_FirstName_Invalid_first_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_FirstName_First_Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_LastName_Invalid_last_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_LastName_Last_Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_BirthDate_Invalid_birth_date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_BirthDate_Birth_Date")]
        public DateTime? BirthDate { get; set; }

        //((\+*)((0[ -]+)*|(91 )*)(\d{12}+|\d{10}+))|\d{5}([- ]*)\d{6}
        //^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$
        //^\\D?(\\d{3})\\D?\\D?(\\d{3})\\D?(\\d{4})$
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid mobile number")]
        //[RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid mobile number format.")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_AddressLine1_Invalid_address_line_1")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_AddressLine1_Address_Line_1")]
        public string AddressLine1 { get; set; }

        //[Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_AddressLine2_Invalid_address_line_2")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_AddressLine2_Address_Line_2")]
        public string AddressLine2 { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_City_Invalid_city_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_City_City")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_State_Invalid_state_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_State_State")]
        public string State { get; set; }

        [Required]
        [DataType(DataType.Text, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_Country_Invalid_country_name")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_Country_Country")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.PostalCode, ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "ExternalLoginConfirmationViewModel_PostalCode_Invalid_postal_code")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "ExternalLoginConfirmationViewModel_PostalCode_Postal_Code")]
        public string PostalCode { get; set; }

        public string Culture { get; set; }

        public string TimeZone { get; set; }

    }
}
