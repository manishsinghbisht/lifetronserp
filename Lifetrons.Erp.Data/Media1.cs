
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Media")]
    [MetadataType(typeof(MediaMetadata))]
    public partial class Media : Entity
    {
        internal sealed class MediaMetadata
        {
            public System.Guid Id { get; set; }
            public string ParentType { get; set; }
            public System.Guid ParentId { get; set; }
            public string MediaType { get; set; }
            public string MediaPath { get; set; }
            public string MediaName { get; set; }
            public string Tags { get; set; }
            public string ShrtDesc { get; set; }
            public string Desc { get; set; }
            public bool IsMarkedAbuse { get; set; }
            public string MarkedAbuseByUserId { get; set; }
            public System.Guid OrgId { get; set; }
            public string CreatedBy { get; set; }
            public System.DateTime CreatedDate { get; set; }
            public string ModifiedBy { get; set; }
            public System.DateTime ModifiedDate { get; set; }
            public bool Authorized { get; set; }
            public bool Active { get; set; }
            public string CustomColumn1 { get; set; }
            public Nullable<System.Guid> ColExtensionId { get; set; }
        }
    }
}
