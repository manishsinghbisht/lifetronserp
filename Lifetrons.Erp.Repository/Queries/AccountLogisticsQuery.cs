using Lifetrons.Erp.Data;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Repository.Queries
{
    public class AccountLogisticsQuery : QueryObject<Account>
    {
        public AccountLogisticsQuery FromCountry(string country)
        {
            And(x => x.ShippingAddressCountry == country);
            return this;
        }

        public AccountLogisticsQuery LivesInCity(string city)
        {
            And(x => x.ShippingAddressCity == city);
            return this;
        }
    }
}