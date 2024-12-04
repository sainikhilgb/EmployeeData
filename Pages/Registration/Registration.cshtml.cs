using EmployeeData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;


namespace EmployeeData.Pages.Registration
{
    public class Registration : PageModel
    {
        private readonly string employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        public List<SelectListItem> GradeOptions { get; set; }
        public List<SelectListItem> GlobalGradeOptions { get; set; }
        public List<SelectListItem> BUOptions { get; set; }
        public List<SelectListItem> BGVOptions { get; set; }

        // OnGet to load dropdown options and initialize the form
        public IActionResult OnGet(string empId)
        {
            // Load dropdown options from the dropdown file
            LoadDropdownOptions();

            if (!string.IsNullOrEmpty(empId))
            {
                // Edit existing employee, load data
                Employee = GetEmployeeById(empId);
                if (Employee == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        // OnPost to save a new employee record or update an existing one
        public async Task<IActionResult> OnPost()
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                LoadDropdownOptions(); // Reload dropdown options if validation fails
                return Page();
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
            

                // Determine the next row
                var rowCount = worksheet.Dimension?.Rows ?? 1;

                if (Employee.EmpId != null) // If editing, update the existing record
                {
                    var existingRow = GetEmployeeRow(worksheet, Employee.EmpId.ToString());
                    if (existingRow != -1)
                    {
                        // Update the existing row with new data
                        var employeeData = GetEmployeeData();
                        int column = 1;
                        foreach (var data in employeeData)
                        {
                            worksheet.Cells[existingRow, column].Value = data.Value;
                            column++;
                        }
                    }
                    else
                    {
                        // If employee not found for editing, append as new
                        AddEmployeeToExcel(worksheet, rowCount);
                    }
                }
                else // If adding a new employee, append as new
                {
                    AddEmployeeToExcel(worksheet, rowCount);
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
                        var globalgrade = worksheet.Cells[row, 9]?.Text?.Trim();
                        var bgv = worksheet.Cells[row, 10]?.Text?.Trim();

                        if (!string.IsNullOrWhiteSpace(grade)) 
                        { 
                            GradeOptions.Add(new SelectListItem { Value = grade, Text = grade });
                        }

                        if (!string.IsNullOrWhiteSpace(bu))
                            BUOptions.Add(new SelectListItem { Value = bu, Text = bu });
                        if (!string.IsNullOrWhiteSpace(bgv))
                            BGVOptions.Add(new SelectListItem { Value = bgv, Text = bgv });

                        if (!string.IsNullOrWhiteSpace(globalgrade)) 
                        { 
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

        private Employee GetEmployeeById(string empId)
        {
            // Try to parse the empId string to an integer
            if (int.TryParse(empId, out int parsedEmpId))
            {
                var employees = GetAllEmployees();
                return employees.FirstOrDefault(emp => emp.EmpId == parsedEmpId); // Compare as integers
            }
            return null; // Return null if empId is not a valid integer
        }

        private int GetEmployeeRow(ExcelWorksheet worksheet, string empId)
        {
            var rowCount = worksheet.Dimension?.Rows ?? 1;
            for (int row = 6; row <= rowCount; row++)
            {
                var cellValue = worksheet.Cells[row, 1].Text; // Assuming EmpId is in column 1
                if (cellValue == empId)
                {
                    return row;
                }
            }
            return -1;
        }

        private Dictionary<string, object> GetEmployeeData()
        {
            return new Dictionary<string, object>
            {
                { "EmpId", Employee.EmpId },
                { "GGID", Employee.GGID },
                { "Resource", Employee.Resource },
                { "Email", Employee.Email },
                { "Gender", Employee.Gender },
                { "DateOfHire", Employee.DateOfHire.ToString("yyyy-MM-dd") },
                { "Grade", Employee.Grade },
                { "GlobalGrade", Employee.GlobalGrade },
                { "BU", Employee.BU },
                { "IsActiveInProject", Employee.IsActiveInProject },
                { "OverallExp", Employee.OverallExp.ToString() },
                { "Skills", Employee.Skills },
                { "Certificates", Employee?.Certificates },
                { "AltriaStartdate", Employee.AltriaStartdate.ToString("yyyy-MM-dd") },
                { "AltriaEnddate", Employee.AltriaEnddate.ToString("yyyy-MM-dd") },
                { "BGVStatus", Employee.BGVStatus },
                { "VISAStatus", Employee.VISAStatus },
            };
        }

        private void AddEmployeeToExcel(ExcelWorksheet worksheet, int rowCount)
        {
            var employeeData = GetEmployeeData();
            int column = 1;
            foreach (var data in employeeData)
            {
                worksheet.Cells[rowCount + 1, column].Value = data.Value;
                column++;
            }
        }

        private List<Employee> GetAllEmployees()
        {
            // This should read from the Excel file and return a list of all employees
            // For simplicity, assuming a method that loads all employees
            var employees = new List<Employee>();
            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                var worksheet = package.Workbook.Worksheets["Employees"];
                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension?.Rows ?? 1;
                    for (int row = 6; row <= rowCount; row++)
                    {
                        var emp = new Employee
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
                        employees.Add(emp);
                    }
                }
            }
            return employees;
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
