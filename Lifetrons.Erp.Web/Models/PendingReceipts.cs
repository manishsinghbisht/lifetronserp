using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Web.Models
{
    public class PendingReceipts
    {
        public string IssuedToProcess { get; set; }
        public string IssuedFromProcess { get; set; }
        public Decimal JobNo { get; set; }
        public Decimal PendingQuantity { get; set; }

    }
}