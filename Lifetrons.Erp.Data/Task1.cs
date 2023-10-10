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

    [Table("Task")]
    [MetadataType(typeof(TaskMetadata))]
    public partial class Task:Entity
    {
        internal sealed class TaskMetadata
        {
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Name_Name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [MaxLength(400)]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Code_Code")]
            public string Code { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ShrtDesc_Description")]
            public string ShrtDesc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Desc_Updates")]
            public string Desc { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_OwnerId_Owner")]
            public string OwnerId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Authorized")]
            public bool Authorized { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Active")]
            public bool Active { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedBy_Created_By")]
            public string CreatedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_CreatedDate_Created_Date")]
            public System.DateTime CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedBy_Modified_By")]
            public string ModifiedBy { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ModifiedDate_Modified_Date")]
            public System.DateTime ModifiedDate { get; set; }


            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_LeadId_Lead")]
            public Nullable<System.Guid> LeadId { get; set; }

            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_ContactId_Contact")]
            public Nullable<System.Guid> ContactId { get; set; }

            
            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_RelatedToObjectName_Related_Area")]
            public string RelatedToObjectName { get; set; }

            
            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_RelatedToId_Related_Record")]
            public Nullable<System.Guid> RelatedToId { get; set; }

           
            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_StartDate_Start_Date")]
            public Nullable<System.DateTime> StartDate { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_EndDate_End_Date")]
            public System.DateTime EndDate { get; set; }

            
            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_IsAllDay_All_Day")]
            public bool IsAllDay { get; set; }
            
            [Required]
            [Display(ResourceType = typeof(Resources.Resources), Name = "Metadata_Status")]
            public System.Guid TaskStatusId { get; set; }

            [Required]
            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_PriorityId_Priority")]
            public System.Guid PriorityId { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_Reminder_Remimder_Date")]
            public System.DateTime Reminder { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_ProgressPercent_ProgressPercent")]
            public Nullable<decimal> ProgressPercent { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_ProgressDesc_Progress_Updates")]
            public string ProgressDesc { get; set; }

            [Display(ResourceType = typeof (Resources.Resources), Name = "TaskMetadata_ReportCompletionToId_Report_Completion_To")]
            public string ReportCompletionToId { get; set; }

        }
    }
}
