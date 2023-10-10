using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("spDepartmentPerformanceComaprisonMonthly_Result")]
    [MetadataType(typeof(spDepartmentPerformanceComaprisonMonthly_ResultMetadata))]
    public partial class spDepartmentPerformanceComaprisonMonthly_Result : Entity, ICloneable
    {
        internal sealed class spDepartmentPerformanceComaprisonMonthly_ResultMetadata
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
