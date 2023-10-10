

using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Table("spTeamOpenWork_Result")]
    [MetadataType(typeof(spTeamOpenWork_ResultMetadata))]
    public partial class spTeamOpenWork_Result : Entity, ICloneable
    {
        internal sealed class spTeamOpenWork_ResultMetadata
        {
            [Key]
            public System.Guid Id { get; set; }
            public string Name { get; set; }
            public System.DateTime Date { get; set; }
            public string Status { get; set; }

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
