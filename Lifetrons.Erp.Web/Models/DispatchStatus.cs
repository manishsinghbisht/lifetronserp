using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Web.Models
{
    public class DispatchStatus
    {
        public DispatchStatus()
        {
            OrderQuantity = 0;
            AlreadyDispatchedQuantity = 0;
            AlreadyDispatchedWeight = 0;
        }

        public Guid OrderLineItem { get; set; }
        public Decimal OrderQuantity { get; set; }
        public Decimal AlreadyDispatchedQuantity { get; set; }
        public Decimal AlreadyDispatchedWeight { get; set; }
    }
}