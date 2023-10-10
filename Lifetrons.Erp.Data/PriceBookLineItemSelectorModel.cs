using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Data
{
    public class PriceBookLineItemSelectorModel
    {
            public Guid ProductId { get; set; }
            public Guid PriceBookId { get; set; }
            public string PriceBookName { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public decimal ListPrice { get; set; }
            public string ShrtDesc { get; set; }
            public string ImageAddr { get; set; }
            public bool IsSelected { get; set; }
            public string ReturnAction { get; set; }
            public string ReturnControl { get; set; }
            public string ReturnUrl { get; set; }

    }
}
