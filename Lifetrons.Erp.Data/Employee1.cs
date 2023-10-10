
namespace Lifetrons.Erp.Data
{
    using Repository.Pattern.Ef6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Employee")]
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee : Entity
    {
        internal sealed class EmployeeMetadata
        {
            [Display(Name = "Name")]
            public string Name { get; set; }

             [Display(Name = "Short Desc")]
            public string ShrtDesc { get; set; }
            public string Designation { get; set; }

             [Display(Name = "Department")]
            public System.Guid DepartmentId { get; set; }

             [Display(Name = "Reports To")]
            public System.Guid ManagerId { get; set; }

             [Display(Name = "Grade")]
            public Nullable<decimal> Grade { get; set; }

             [Display(Name = "Date Of Joining")]
            public System.DateTime DOJ { get; set; }

             [Display(Name = "Date Of Left")]
            public Nullable<System.DateTime> DOL { get; set; }

             [Display(Name = "PF Flag")]
            public bool PFFlag { get; set; }

             [Display(Name = "ESI Flag")]
            public bool ESIFlag { get; set; }

             [Display(Name = "Food Flag")]
            public bool FoodFlag { get; set; }

             [Display(Name = "Food Rate")]
            public Nullable<decimal> FoodRate { get; set; }

             [Display(Name = "Hourly Rate")]
             [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> HourlyRate { get; set; }

             [Display(Name = "Is Employed")]
            public bool Employed { get; set; }

             [Display(Name = "Has Left")]
            public bool HasLeft { get; set; }

             [Display(Name = "Basic")]
             [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> Basic { get; set; }

             [Display(Name = "HRA")]
            public Nullable<decimal> HRA { get; set; }

             [Display(Name = "Executive Allowance")]
            public Nullable<decimal> ExecA { get; set; }

             [Display(Name = "Conveyance Allowance")]
            public Nullable<decimal> ConvA { get; set; }

             [Display(Name = "Tea Allowance")]
            public Nullable<decimal> TeaA { get; set; }

             [Display(Name = "Wash Allowance")]
            public Nullable<decimal> WashA { get; set; }

             [Display(Name = "CE Allowance")]
            public Nullable<decimal> CEA { get; set; }

             [Display(Name = "Othe Allowance")]
            public Nullable<decimal> OthA { get; set; }

             [Display(Name = "PF (Fix)")]
            public Nullable<decimal> PFFix { get; set; }

             [Display(Name = "Car Installment")]
            public Nullable<decimal> CarLInst { get; set; }

            [Display(Name = "Income Tax Installment")]
            public Nullable<decimal> IncomeTAXInst { get; set; }

            [Display(Name = "Professional Tax Installment")]
            public Nullable<decimal> ProfTAXInst { get; set; }

            [Display(Name = "Education Tax Installment")]
            public Nullable<decimal> EduTAXInst { get; set; }

            [Display(Name = "Service Tax Installment")]
            public Nullable<decimal> ServiceTAXInst { get; set; }

            [Display(Name = "Other Tax Installment")]
            public Nullable<decimal> OtherTAXInst { get; set; }

            [Display(Name = "Life Insurance Installment")]
            public Nullable<decimal> LifeInsuInst { get; set; }

            [Display(Name = "Health Insurance Installment")]
            public Nullable<decimal> HealthInsuInst { get; set; }

            [Display(Name = "Loan (With Interest) Installment")]
            public Nullable<decimal> LoanWInst { get; set; }

            [Display(Name = "Loan (Without Interest) Installment")]
            public Nullable<decimal> LoanWOInst { get; set; }

            [Display(Name = "Provident Fund No")]
            public string PFNo { get; set; }

            [Display(Name = "ESI No")]
            public string ESINo { get; set; }

            [Display(Name = "Bank Name / Details")]
            public string Bank { get; set; }

            [Display(Name = "Bank A/C No")]
            public string BankAcNo { get; set; }

            [Display(Name = "Life Insurance No.")]
            public string LifeInsuNo { get; set; }

            [Display(Name = "Health Insurance No.")]
            public string HealthInsuNo { get; set; }

            [Display(Name = "Gratuity No.")]
            public string GratuityNo { get; set; }

            [Display(Name = "PAN")]
            public string PAN { get; set; }

            public string EmpVend { get; set; }

            [Display(Name = "DISP No")]
            public string DISPNo { get; set; }

            [Display(Name = "Last Performance Review Date")]
            public Nullable<System.DateTime> LastPerformanceReviewDate { get; set; }

            [Display(Name = "Next Performance Review Date")]
            public Nullable<System.DateTime> NextPerformanceReviewDate { get; set; }

            [Display(Name = "Last Compensation Review Date")]
            public Nullable<System.DateTime> LastCompensationReviewDate { get; set; }

            [Display(Name = "Next Compensation Review Date")]
            public Nullable<System.DateTime> NextCompensationReviewDate { get; set; }
        }
    }
}
