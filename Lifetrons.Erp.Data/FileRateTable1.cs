
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FileRateTable")]
    [MetadataType(typeof(FileRateTableMetadata))]
    public partial class FileRateTable : Entity
    {
        internal sealed class FileRateTableMetadata
        {
            public System.Guid Id { get; set; }
            public string FileType { get; set; }
            public System.Guid AccountId { get; set; }
            public System.Guid SubAccountId { get; set; }
            public string RateType { get; set; }
            public Nullable<decimal> Rate { get; set; }
            public System.DateTime StartDate { get; set; }
            public Nullable<System.DateTime> EndDate { get; set; }
            public System.Guid OrgId { get; set; }
            public string CreatedBy { get; set; }
            public System.DateTime CreatedDate { get; set; }
            public string ModifiedBy { get; set; }
            public System.DateTime ModifiedDate { get; set; }
            public bool Authorized { get; set; }
            public bool Active { get; set; }
            public string CustomColumn1 { get; set; }
            public Nullable<System.Guid> ColExtensionId { get; set; }
            public byte[] TimeStamp { get; set; }
            public string TemplateUrl { get; set; }
            public Nullable<System.DateTime> TemplateUpdateDate { get; set; }
            public string TemplateName { get; set; }
            public Nullable<System.Guid> ContactId { get; set; }
            public string ShrtDesc { get; set; }
            public string Desc { get; set; }

            //public virtual Account Account { get; set; }
            //public virtual Account Account1 { get; set; }
            //public virtual AspNetUser AspNetUser { get; set; }
            //public virtual AspNetUser AspNetUser1 { get; set; }
            //public virtual Organization Organization { get; set; }
        }
    }
}
