using EmployeeData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;  // EPPlus library to read Excel files
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeData.Pages.Registration
{
    public class EmployeeList : PageModel
    {
        [BindProperty]
        public Employee employees { get; set; } = new Employee();
        [BindProperty]
        public Project project { get; set; } = new Project();

        public List<SelectListItem> GradeOptions { get; set; }
         public List<SelectListItem> GlobalGradeOptions { get; set; }
        public List<SelectListItem> BUOptions { get; set; }
        public List<SelectListItem> BGVOptions { get; set; }
        public List<SelectListItem> ProjectCodeOptions { get; set; }
        public List<SelectListItem> ProjectNameOptions { get; set; }
        public List<SelectListItem> PODNameOptions { get; set; }
        public List<SelectListItem> OffShoreCityOptions { get; set; }
        public List<SelectListItem> TypeOptions { get; set; }
        public List<SelectListItem> TowerOptions { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Project> Projects { get; set; } = new List<Project>();
        private readonly string employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");

        private Dictionary<string, string> projectCodeToNameMapping = new Dictionary<string, string>();


        // Method to load Employee and Project details from Excel
        public void OnGet()
        {
           LoadDropdownOptions();
            LoadEmployeeData();
            LoadProjectData();

                     // Apply search filter if a SearchTerm is provided and is a valid EmpId
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                if (int.TryParse(SearchTerm, out int searchEmpId))
                {
                    // If it's a valid integer, filter the employees based on EmpId
                    Employees = Employees.Where(e => e.EmpId.ToString().Contains(SearchTerm)).ToList();
                }
            }
        }

        public async Task<IActionResult> OnPost()
{
    // Validate model
    if (!ModelState.IsValid)
    {
        LoadDropdownOptions(); // Reload dropdown options if validation fails
    List<string> validationErrors = new List<string>();
        foreach (var key in ModelState.Keys)
        {
            foreach (var error in ModelState[key].Errors)
            {
                validationErrors.Add(error.ErrorMessage);
            }
        }

        // Return a response with the validation errors
        return BadRequest(new { errors = validationErrors });
    }

    try
    {
        // Ensure ExcelPackage licensing
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        // Define the file path
        string employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");

        // Ensure directory exists
        string directory = Path.GetDirectoryName(employeeFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Check if the file exists, or create a new one
        bool isNewFile = !System.IO.File.Exists(employeeFilePath);
        using var package = new ExcelPackage(new FileInfo(employeeFilePath));

        // Load or create the worksheet
        var worksheet = package.Workbook.Worksheets["Employees"];
        // if (worksheet == null)
        // {
        //     worksheet = package.Workbook.Worksheets.Add("Employees");

        //     // // Define headers
        //     // string[] headers = new[]
        //     // {
        //     //     "EmpId", "GGID", "Resource", "Email", "Gender", "DateOfHire", "Grade", "GlobalGrade", "BU",
        //     //     "IsActiveInProject", "OverallExp", "Skills", "Certificates","AltriaStartdate","AltriaEnddate",
        //     //      "BGVStatus","VISAStatus","ProjectCode", "ProjectName", "PONumber", "PODName", "StartDate", 
        //     //     "EndDate","Location","OffshoreCity", "OffshoreBackup", "AltriaPODOwner", "ALCSDirector", "Type", 
        //     //     "Tower", "ABLGBL", "TLName",  "SL","New", "Transition", "COR", "Group", "RoleinPOD", "MonthlyPrice"
        //     // };

         
        //     //  // Add dynamic months to headers
        //     // var month = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        //     // headers = headers.Concat(month).ToArray();

        //     // // Populate header row
        //     // for (int i = 0; i < headers.Length; i++)
        //     // {
        //     //     worksheet.Cells[1, i + 1].Value = headers[i];
        //     // }
        // }

        // Determine the next row
        var rowCount = worksheet.Dimension?.Rows ?? 1;

        // Prepare employee data to insert
        var employeeData = new Dictionary<string, object>
        {
            { "EmpId", employees.EmpId },
            { "GGID", employees.GGID },
            { "Resource", employees.Resource },
            { "Email", employees.Email },
            { "Gender", employees.Gender },
            { "DateOfHire", employees.DateOfHire.ToString("yyyy-MM-dd") },
            { "Grade", employees.Grade },
            { "GlobalGrade", employees.GlobalGrade },
            { "BU", employees.BU },
            { "IsActiveInProject", employees.IsActiveInProject },
            { "OverallExp", employees.OverallExp.ToString() },
            { "Skills", employees.Skills },
            { "Certificates", employees?.Certificates },
            { "AltriaStartdate", employees.AltriaStartdate.ToString("yyyy-MM-dd") },
            { "AltriaEnddate", employees.AltriaEnddate.ToString("yyyy-MM-dd") },
            { "BGVStatus", employees.BGVStatus },
            { "VISAStatus", employees.VISAStatus },
            { "ProjectCode", project.ProjectCode },
            { "ProjectName", project.ProjectName },
            { "PONumber", project.PONumber },
            { "PODName", project.PODName },
            { "StartDate", project.StartDate.ToString("yyyy-MM-dd") },
            { "EndDate", project.EndDate.ToString("yyyy-MM-dd") },
            { "Location", project.Location },
            { "OffshoreCity", project.OffshoreCity },
            { "OffshoreBackup", project.OffshoreBackup },
            { "AltriaPODOwner", project.AltriaPODOwner },
            { "ALCSDirector", project.ALCSDirector },
            { "Type", project.Type },
            { "Tower", project.Tower },
            { "ABLGBL", project.ABLGBL },
            { "TLName", project.TLName },
            { "Transition", project.Transition },
            { "COR", project.COR },
            { "Group", project.Group },
            { "RoleinPOD", project.RoleinPOD },
            { "MonthlyPrice", project.MonthlyPrice.ToString() }
        };
            
        


        // Insert data into the worksheet
        int column = 1;
        foreach (var data in employeeData)
        {
            worksheet.Cells[rowCount + 1, column].Value = data.Value;
            column++;
        }

        // Save the changes to the file
        await package.SaveAsync();

        // Redirect to employee list
        return RedirectToPage("/Registration/EmployeeList");
    }
    catch (Exception ex)
    {
        // Log and display error
        Console.WriteLine($"Error: {ex.Message}");
        ModelState.AddModelError("", "An error occurred while processing the request.");
        LoadDropdownOptions(); // Reload dropdowns
        return Page();
    }
}
private void LoadDropdownOptions()
        {
            GradeOptions = new List<SelectListItem>();
            GlobalGradeOptions = new List<SelectListItem>();
            BUOptions = new List<SelectListItem>();
            BGVOptions = new List<SelectListItem>();
            ProjectCodeOptions = new List<SelectListItem>();
            ProjectNameOptions = new List<SelectListItem>();
            PODNameOptions = new List<SelectListItem>();
            OffShoreCityOptions = new List<SelectListItem>();
            TypeOptions = new List<SelectListItem>();
            TowerOptions = new List<SelectListItem>();


            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                var worksheet = package.Workbook.Worksheets["Dropdown"]; // Ensure this matches your worksheet name
                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var grade = worksheet.Cells[row, 1]?.Text?.Trim();
                        var bu = worksheet.Cells[row, 2]?.Text?.Trim();
                        var projectcode = worksheet.Cells[row, 3]?.Text?.Trim();
                        var projectname = worksheet.Cells[row, 4]?.Text?.Trim();
                        var PODname = worksheet.Cells[row, 5]?.Text?.Trim();
                        var Offshore = worksheet.Cells[row, 6]?.Text?.Trim();
                        var type = worksheet.Cells[row, 7]?.Text?.Trim();
                        var tower = worksheet.Cells[row, 8]?.Text?.Trim();
                        var globalgrade = worksheet.Cells[row, 9]?.Text?.Trim();
                        var bgv = worksheet.Cells[row, 10]?.Text?.Trim();

                        
                        if (!string.IsNullOrWhiteSpace(projectcode) && !string.IsNullOrWhiteSpace(projectname))
                        {
                            ProjectCodeOptions.Add(new SelectListItem { Value = projectcode, Text = projectcode });
                            projectCodeToNameMapping.Add(projectcode, projectname);
                        }
                        if (!string.IsNullOrWhiteSpace(PODname))
                        {
                            PODNameOptions.Add(new SelectListItem { Value = PODname, Text = PODname });
                        }
                        if (!string.IsNullOrWhiteSpace(Offshore))
                        {
                            OffShoreCityOptions.Add(new SelectListItem { Value = Offshore, Text = Offshore });
                        }
                        if (!string.IsNullOrWhiteSpace(type))
                        {
                            TypeOptions.Add(new SelectListItem { Value = type, Text = type });
                        }
                        if (!string.IsNullOrWhiteSpace(tower))
                        {
                            TowerOptions.Add(new SelectListItem { Value = tower, Text = tower });
                        }

                        if (!string.IsNullOrWhiteSpace(grade)){ 

                            GradeOptions.Add(new SelectListItem { Value = grade, Text = grade });
                        }

                        if (!string.IsNullOrWhiteSpace(bu))
                            BUOptions.Add(new SelectListItem { Value = bu, Text = bu });
                        if (!string.IsNullOrWhiteSpace(bgv))
                            BGVOptions.Add(new SelectListItem { Value = bgv, Text = bgv });

                         if (!string.IsNullOrWhiteSpace(globalgrade)){ 

                            GlobalGradeOptions.Add(new SelectListItem { Value = globalgrade, Text = globalgrade });
                        }
                        
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Worksheet 'Dropdown' not found in the dropdown file.");
                }
            }
            else
            {
                ModelState.AddModelError("", $"Dropdown file not found at {employeeFilePath}.");
            }
        }


       
        private void LoadEmployeeData()
        {
            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                var worksheet = package.Workbook.Worksheets["Employees"];
                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension.Rows;

                    // Loop through the rows and add employees
                    for (int row = 6; row <= rowCount; row++)
                    {
                        var employee = new Employee
                        {
                            EmpId = ParseInt(worksheet.Cells[row, 15].Text),
                            GGID = ParseInt(worksheet.Cells[row, 14].Text),
                            Resource = worksheet.Cells[row, 17].Text,
                            Email = worksheet.Cells[row, 16].Text,
                            Gender = worksheet.Cells[row, 21].Text,
                            DateOfHire = ParseDate(worksheet.Cells[row, 6].Text),
                            Grade = worksheet.Cells[row, 18].Text,
                            GlobalGrade = worksheet.Cells[row, 19].Text,
                            BU = worksheet.Cells[row, 4].Text,
                            IsActiveInProject = worksheet.Cells[row, 20].Text,
                            OverallExp = ParseInt(worksheet.Cells[row, 27].Text),
                            Skills = worksheet.Cells[row, 28].Text,
                            Certificates = worksheet.Cells[row, 33].Text,
                            AltriaStartdate = ParseDate(worksheet.Cells[row, 127].Text),
                            AltriaEnddate = ParseDate(worksheet.Cells[row, 128].Text),
                            BGVStatus = worksheet.Cells[row, 129].Text,
                            VISAStatus = worksheet.Cells[row, 130].Text,
                        
                    };

                        Employees.Add(employee);
                    }
                }
            }
        }

        // Method to load Project data from Excel file
        private void LoadProjectData()
        {
            var package = new ExcelPackage(new FileInfo(employeeFilePath));
            var worksheet = package.Workbook.Worksheets["Employees"];
            
         // Assuming Project data is in the second sheet
                if(worksheet!= null){
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 6; row <= rowCount; row++) // Skip header row
                {
                    
                    var project = new Project
                    {
                        ProjectCode = ParseInt(worksheet.Cells[row, 7].Text),
                        ProjectName = worksheet.Cells[row, 8].Text,
                        PONumber = ParseInt(worksheet.Cells[row, 9].Text),
                        PODName = worksheet.Cells[row, 10].Text,
                        StartDate = ParseDate(worksheet.Cells[row, 131].Text),
                        EndDate = ParseDate(worksheet.Cells[row, 132].Text),
                        Location = worksheet.Cells[row, 22].Text,
                        OffshoreCity = worksheet.Cells[row, 23].Text,
                        OffshoreBackup = worksheet.Cells[row, 34].Text,
                        AltriaPODOwner = worksheet.Cells[row, 12].Text, 
                        ALCSDirector = worksheet.Cells[row, 13].Text,
                        Type = worksheet.Cells[row, 1].Text,
                        Tower = worksheet.Cells[row, 2].Text,
                        ABLGBL = worksheet.Cells[row, 3].Text,
                        TLName = worksheet.Cells[row, 5].Text,
                        Transition = worksheet.Cells[row, 35].Text,
                        COR = worksheet.Cells[row, 60].Text,
                        Group = worksheet.Cells[row, 62].Text,
                        RoleinPOD = worksheet.Cells[row, 25].Text,
                        MonthlyPrice = ParseDecimal(worksheet.Cells[row, 63].Text),
                        January = ParseDecimal(worksheet.Cells[row,36].Text),
                        February = ParseDecimal(worksheet.Cells[row,37].Text),
                        March = ParseDecimal(worksheet.Cells[row,38].Text),
                        April = ParseDecimal(worksheet.Cells[row,39].Text),
                        May = ParseDecimal(worksheet.Cells[row,40].Text),
                        June = ParseDecimal(worksheet.Cells[row,41].Text),
                        July = ParseDecimal(worksheet.Cells[row,42].Text),
                        August = ParseDecimal(worksheet.Cells[row,43].Text),
                        September = ParseDecimal(worksheet.Cells[row,44].Text),
                        October = ParseDecimal(worksheet.Cells[row,45].Text),
                        November = ParseDecimal(worksheet.Cells[row,46].Text),
                        December =ParseDecimal(worksheet.Cells[row,47].Text),
                    };

                    

                    Projects.Add(project);  // Add project to the Projects list
                    }
                }
        }
            
        

// Method to delete an employee
[HttpDelete]
public IActionResult OnPostDelete(int empId)
{
    // Remove employee from the in-memory list
    var employeeToDelete = Employees.FirstOrDefault(e => e.EmpId == empId)?? new Employee();
    if (employeeToDelete != null)
    {
        Employees.Remove(employeeToDelete);

        // Update the Excel file
        DeleteEmployeeFromExcel(empId);

        // Reload data to reflect changes
        LoadEmployeeData();
    }

    // Redirect back to the same page after deletion
    return RedirectToPage();
}

// Method to delete an employee from the Excel file
private void DeleteEmployeeFromExcel(int empId)
{
    if (System.IO.File.Exists(employeeFilePath))
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(new FileInfo(employeeFilePath)))
        {
            var worksheet = package.Workbook.Worksheets["Employees"];
            if (worksheet != null)
            {
                var rowCount = worksheet.Dimension.Rows;
                for (int row = 6; row <= rowCount; row++) // Skip header
                {
                    int currentEmpId = ParseInt(worksheet.Cells[row, 1].Text);
                    if (currentEmpId == empId)
                    {
                        worksheet.DeleteRow(row);
                        break;
                    }
                }

                // Save changes to the Excel file
                package.Save();
            }
        }
    }
}
        [HttpGet]
        public IActionResult OnGetProjectName(string projectCode)
        {
            LoadDropdownOptions();
            if (string.IsNullOrWhiteSpace(projectCode))
                return new JsonResult("Invalid Project Code");

            if (projectCodeToNameMapping.TryGetValue(projectCode, out var projectName))
                return new JsonResult(projectName);

            return new JsonResult("Project Code not found");
        }

              private DateTime ParseDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out var date))
            {
                return date;
            }
            return DateTime.MinValue; // Default value for invalid or missing dates
        }

        private int ParseInt(string numberString)
        {
            if (int.TryParse(numberString, out var number))
            {
                return number;
            }
            return 0; // Default value for invalid or missing numbers
        }

        private decimal ParseDecimal(string numberString)
        {
            if (decimal.TryParse(numberString, out var number))
            {
                return number;
            }
            return 0; // Default value for invalid or missing numbers
        }

        
    }
}
