using System;
using System.Linq;
using System.Linq.Expressions;
using Lifetrons.Erp.Data;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Repository.Queries
{
    public class OrderSalesQuery : QueryObject<Order>
    {
        public decimal Amount { get; set; }
        public string Country { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override Expression<Func<Order, bool>> Query()
        {
            return (x =>
                x.OrderLineItems.Sum(y => y.LineItemAmount) > Amount &&
                x.DeliveryDate >= FromDate &&
                x.DeliveryDate <= ToDate &&
                x.ShippingAddressCountry == Country);
        }
    }
}