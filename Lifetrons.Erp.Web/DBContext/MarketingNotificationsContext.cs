using System.Data.Entity;
using Lifetrons.Erp.Web.Models;

namespace Lifetrons.Erp.Web.DBContext
{
    public class MarketingNotificationsContext : DbContext
    {
        public MarketingNotificationsContext()
            : base("MarketingNotificaitonsConnection") { }

        public DbSet<Subscriber> Subscribers { get; set; }
    }
}