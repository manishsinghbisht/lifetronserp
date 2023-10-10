

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using System;
    using System.Collections.Generic;

    [Table("AspNetRole")]
    [MetadataType(typeof(AspNetRole))]
    public partial class AspNetRole : Entity
    {
      internal sealed class AspNetRoleMetadata
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
