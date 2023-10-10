using System.Linq;
using Lifetrons.Erp.Data;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Repository.Queries
{
    public class AccountSalesQuery : QueryObject<Account>
    {
        public AccountSalesQuery WithPurchasesMoreThan(decimal amount)
        {
            And(x => x.Orders
                .SelectMany(y => y.OrderLineItems)
                .Sum(z => z.SalesPrice * z.Quantity) > amount);

            return this;
        }

        public AccountSalesQuery WithQuantitiesMoreThan(decimal quantity)
        {
            And(x => x.Orders
                .SelectMany(y => y.OrderLineItems)
                .Sum(z => z.Quantity) > quantity);

            return this;
        }
    }
}