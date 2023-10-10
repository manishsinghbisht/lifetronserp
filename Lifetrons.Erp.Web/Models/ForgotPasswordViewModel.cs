using System.ComponentModel.DataAnnotations;

namespace Lifetrons.Erp.Web.Models
{

    public class ForgotPasswordViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    
}