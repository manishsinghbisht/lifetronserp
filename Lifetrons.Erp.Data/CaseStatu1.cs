﻿using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CaseStatu")]
    [MetadataType(typeof(CaseStatuMetadata))]
    public partial class CaseStatu : Entity
    {
        internal sealed class CaseStatuMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(4)]
            public string Code { get; set; }

        }
    }
}
