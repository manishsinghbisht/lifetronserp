using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Web.Models
{
    public class Search
    {
       public string Action { get; set; }

       public string Controller { get; set; }

       public string Placeholder { get; set; }
    }
}