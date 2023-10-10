using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Web.Models
{
    public class ProductionLoad
    {
        public string Process { get; set; }
        public Decimal JobNo { get; set; }
        public Decimal Quantity { get; set; }

    }
}