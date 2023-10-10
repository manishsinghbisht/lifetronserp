
using System;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
   using Repository;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("File")]
    [MetadataType(typeof(FileMetadata))]
    public partial class File : Entity
    {
        internal sealed class FileMetadata
        {
            public System.Guid Id { get; set; }
            public string FileType { get; set; }
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public string FileCode { get; set; }
            public string FileRefNo { get; set; }
            public string Tags { get; set; }

            [Display(Name = "Recieved Page Count")]
            [Range(0, 25, ErrorMessage = "Please enter valid number")]
            public Nullable<decimal> NumberOfPagesRecieved { get; set; }

            [Display(Name = "Submitted Page Count")]
            [Range(0, 25, ErrorMessage = "Please enter valid number")]
            public Nullable<decimal> NumberOfPagesSubmitted { get; set; }

            public string ShrtDesc { get; set; }
            public string Desc { get; set; }
            public string Status { get; set; }
            public Nullable<bool> IsVerified { get; set; }
            public string RateType { get; set; }
            public Nullable<decimal> Rate { get; set; }
            public Nullable<System.Guid> AccountId { get; set; }
            public Nullable<System.Guid> SubAccountId { get; set; }
            public string Origin { get; set; }
            public string OriginCode { get; set; }

            [DataType(DataType.EmailAddress)]
            public string OriginSenderEmail { get; set; }
            [DataType(DataType.EmailAddress)]
            public string OriginApproverEmail { get; set; }
            [DataType(DataType.EmailAddress)]
            public string ProcessorEmail { get; set; }
            public string ProcessorId { get; set; }
            [DataType(DataType.EmailAddress)]
            public string ApproverEmail { get; set; }
            public string ApproverId { get; set; }
            public Nullable<bool> IsApproved { get; set; }
            public string BackupEmail { get; set; }
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
            public string SubmittedFilePath { get; set; }
            public string SubmittedFileName { get; set; }
            public Nullable<System.DateTime> SubmittedDate { get; set; }
            public Nullable<System.DateTime> DeliveredDate { get; set; }
            public string FileExtension { get; set; }
            public Nullable<System.DateTime> FileSentDate { get; set; }
            public string RcvdFileEmailSubject { get; set; }
            public string RcvdFileEmailBody { get; set; }
            public string SubmittedFileEmailBody { get; set; }
            public string DeliveredFileEmailSubject { get; set; }
            public string TemplateName { get; set; }
            public Nullable<System.Guid> ContactId { get; set; }
            public string OriginCCEmails { get; set; }
            public string ProcessorCCEmails { get; set; }


            //public virtual Account Account { get; set; }
            //public virtual Account Account1 { get; set; }
            //public virtual AspNetUser AspNetUserApprover { get; set; }
            //public virtual AspNetUser AspNetUser1 { get; set; }
            //public virtual AspNetUser AspNetUser2 { get; set; }
            //public virtual AspNetUser AspNetUserProcessor { get; set; }
            //public virtual Organization Organization { get; set; }
        }
    }
}
