using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeData.Models
{
    public class Project
    {
     [DisplayName("Project Code")]
         public int ProjectCode {get; set;}

        [DisplayName("Project name")]
        public string? ProjectName {get; set;}
        [Range(0,10,ErrorMessage ="Value Should be in between 0-10")]
        public int PONumber {get;set;}
        [Required(ErrorMessage = "Please Select the POD name")]
        [DisplayName("POD name")]
        public string PODName {get; set;}
        [Required(ErrorMessage = "Please Enter the Altria POD name")]
        public string AltriaPODOwner {get; set;}
        [Required(ErrorMessage = "Please Enter the ALCS Director")]
        public string ALCSDirector {get; set;}
        [Required(ErrorMessage = "Please Select the Type")]
        
        public string Type {get;set;}
        [Required(ErrorMessage = "Please Select the Tower")]
        public string Tower {get;set;}
        [Required(ErrorMessage = "Please Select the ABL or GBL")]
        public string? ABLGBL {get;set;}
        [Required(ErrorMessage = "Please Select the POD name")]
        public string TLName {get;set;}

        [Required(ErrorMessage = "Please select Onshore or Offshore")]
        public string Location {get; set;}
        [DisplayName("Offshore City")]
        public string OffshoreCity {get; set;}
        [DisplayName("Offshore Backup")]
        public string OffshoreBackup {get; set;}

        public string Transition {get; set;}
        
        [Required(ErrorMessage = "Please enter the Project Start date")]
        [DataType(DataType.Date)]
        [DisplayName("Start date")]
         public DateTime StartDate {get; set;}

        [Required(ErrorMessage = "Please enter the Project end date")]
        [DataType(DataType.Date)]
        [DisplayName("End date")]
         public DateTime EndDate {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal January {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal February {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal March {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal April {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal May {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal June {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal July {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal August {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal September {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal October {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal November {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal December {get; set;}

         public string COR {get; set;}

         public string Group {get; set;}
         [DisplayName("Monthly Price")]
        public decimal MonthlyPrice {get; set;} 
       
        [DisplayName("Role in POD")]
        public string RoleinPOD {get; set;}


    }
}