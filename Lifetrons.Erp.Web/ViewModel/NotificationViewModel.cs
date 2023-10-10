using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Lifetrons.Erp.Web.ViewModel
{
    public class NotificationViewModel
    {
        [Required]
        public string Message { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
    }
}