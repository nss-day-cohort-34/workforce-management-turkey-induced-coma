using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models
{
    public class Department
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Department Name")]
        public string Name { get; set; }
        public int Budget { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
