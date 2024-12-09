// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getDate() {
    // Get the current date
    const today = new Date();
    // Format the date as YYYY-MM-DD
    const formattedDate = today.toISOString().split('T')[0];
     // Select all elements with the class 'date-field'
     const dateFields = document.querySelectorAll('.date-field');
     // Loop through each date field and set the value
     dateFields.forEach(field => {
         field.value = formattedDate;
     });
}
window.onload = getDate;

function fetchProjectName(projectCode) {
  $.ajax({
      url: `Registration?handler=ProjectName&projectCode=${projectCode}`,
      type: "GET",
      success: function(projectName) {
          $("#ProjectName").val(projectName);
      },
      error: function(error) {
          console.error("Error fetching project name:", error);
      }
  });
}

function fetchGlobalGrade(grade) {
    $.ajax({
        url: `Registration?handler=GlobalGrade&Grade=${grade}`,
        type: "GET",
        success: function(GlobalGrade) {
            $("#GlobalGrade").val(GlobalGrade);
        },
        error: function(error) {
            console.error("Error fetching project name:", error);
        }
    });
  }

  
  function editEmployee(empId) {
    $.ajax({
        url: `Registration/Registration?handler=empId=${empId}`,
        type: "GET",
        success: function(GlobalGrade) {
            $("#GlobalGrade").val(GlobalGrade);
        },
        error: function(error) {
            console.error("Error fetching project name:", error);
        }
    });
    window.location.href = `http://localhost:5165/Registration/Registration?handler=empId=${empId}`;
}