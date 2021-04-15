using System.ComponentModel.DataAnnotations;

namespace Project.WebAPI.Dtos
{
    public class EmployeeDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Employee Name must contain a minimum of 4 characters and a maximum of 100 characters")]
        public string Employee_Name { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Range(0, int.MaxValue)]
        public int Employee_Salary { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Range(0, 120, ErrorMessage = "Employee_Age cannot live more than 120 years xD")]
        public string Employee_Age { get; set; }
    }
}
