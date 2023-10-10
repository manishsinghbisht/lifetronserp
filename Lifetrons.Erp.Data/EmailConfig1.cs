

using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmailConfig")]
    [MetadataType(typeof(EmailConfigMetadata))]
    public partial class EmailConfig : Entity
    {
        internal sealed class EmailConfigMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(128)]
            public string UserId { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(25)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string SmtpUsername { get; set; }
            
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string SmtpPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string SmtpHost { get; set; }

            [Required]
            public Nullable<int> SmtpPort { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Pop3Username { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Pop3Password { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string Pop3Host { get; set; }

            [Required]
            public Nullable<int> Pop3Port { get; set; }

        }
    }
}
