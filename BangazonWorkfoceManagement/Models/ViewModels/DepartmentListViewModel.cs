using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class DepartmentListViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Department Name")]
        public string Name { get; set; }
        public int Budget { get; set; }
        [Display(Name = "Employee Count")]
        public int  EmployeeCount { get; set; }
    }
}
