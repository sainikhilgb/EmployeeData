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

function fetchProjectName() {
  const projectCodeSelect = document.getElementById("projectCode");
  const selectedOption = projectCodeSelect.options[projectCodeSelect.selectedIndex].value;
  console.log(selectedOption)

  const currentPage = window.location.pathname;
  const handlerUrl = currentPage.includes("EmployeeList")
    ? `?handler=ProjectName&projectCode=${selectedOption}`
    : `?handler=ProjectName&projectCode=${selectedOption}`;

  $.ajax({
    url: handlerUrl,
    type: "GET",
    success: function(projectName) {
      $("#projectName").val(projectName);
    },
    error: function(jqXHR, textStatus, errorThrown) {
      console.error("Error fetching project name:", textStatus, errorThrown);
    }
  });
}


  
document.getElementById('add-btn').addEventListener('click', function(event) {
  event.preventDefault();

  const tableBody = document.getElementById('project-details');
  const templateRow = document.getElementById('template-row');

  // Clone the template row
  const newRow = templateRow.cloneNode(true);
  newRow.id = ''; // Remove the id to avoid conflicts
  newRow.classList.remove('d-none'); // Make the row visible

  // Append the new row to the table body
  tableBody.appendChild(newRow);

  // Add event listener to the delete button in the new row
  const deleteBtn = newRow.querySelector('.delete-row-btn');
  deleteBtn.addEventListener('click', function() {
      tableBody.removeChild(newRow);
  });
});