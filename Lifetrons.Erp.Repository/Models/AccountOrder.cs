using System;

namespace Lifetrons.Erp.Repository.Models
{
    public class AccountOrder
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}