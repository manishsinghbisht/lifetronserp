using System;

namespace Lifetrons.Erp.Data
{
    public class StockItemQuantityTotals
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public Decimal TotalQuantity { get; set; }
        public Decimal Value { get; set; }
    }
}