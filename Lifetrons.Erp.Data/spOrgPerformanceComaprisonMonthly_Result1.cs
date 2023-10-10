using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("spOrgPerformanceComaprisonMonthly_Result")]
    [MetadataType(typeof(spOrgPerformanceComaprisonMonthly_ResultMetadata))]
    public partial class spOrgPerformanceComaprisonMonthly_Result : Entity, ICloneable
    {
        internal sealed class spOrgPerformanceComaprisonMonthly_ResultMetadata
        {
            [Key]
            public System.Guid Id { get; set; }
            public string Name { get; set; }
            public System.DateTime Date { get; set; }
            public string Status { get; set; }
            public Nullable<decimal> LineItemsAmount { get; set; }
            public Nullable<decimal> LineItemsQuantity { get; set; }

        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
