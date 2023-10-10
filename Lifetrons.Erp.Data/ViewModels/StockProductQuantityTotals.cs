using System;

namespace Lifetrons.Erp.Data
{
    public class StockProductQuantityTotals
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Decimal TotalQuantity { get; set; }
        public Decimal Value { get; set; }
    }
}