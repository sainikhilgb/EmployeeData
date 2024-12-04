using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeData.Models 
{
    public class Employee 
    {
        [Key]
        [Required(ErrorMessage = "Please enter EmployeeId")]
        public int EmpId {get; set;}
        [Required(ErrorMessage = "Please enter GGID")]
        public int GGID {get; set;}

        [Required(ErrorMessage = "Please enter your first name")]
        [StringLength(50)]
        public string Resource {get; set;}

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress]  
        public string Email {get; set;}
        [Required(ErrorMessage = "Please Select your Grade")]
        public string Grade {get; set;}
        public string GlobalGrade {get; set;}
        [Required(ErrorMessage = "Please enter the date of hire")]
        [DataType(DataType.Date)]
        [DisplayName("Date Of Hire")]
         public DateTime DateOfHire {get; set;}
    
        [Required(ErrorMessage = "Select Yes for tagging/working in project")]
        public string IsActiveInProject { get; set; }
        [Required(ErrorMessage = "Please Select the BU")]
         public string BU {get; set;}

        public string Gender { get; set; }

        public decimal OverallExp {get; set;}
        public string Skills {get; set;}

        
        public string Certificates {get; set;}

        [Required(ErrorMessage = "Please enter the Altria Start date")]
        [DataType(DataType.Date)]
        [DisplayName("Altria Start date")]
         public DateTime AltriaStartdate {get; set;}

        [Required(ErrorMessage = "Please enter the Altria End date")]
        [DataType(DataType.Date)]
        [DisplayName("Altria End date")]
         public DateTime AltriaEnddate {get; set;}

         [Required(ErrorMessage = "Please select BGV Status")]
         public string BGVStatus {get; set;}

         [Required(ErrorMessage = "Please select VISA Status")]
         public string VISAStatus {get; set;}

        
    }
}