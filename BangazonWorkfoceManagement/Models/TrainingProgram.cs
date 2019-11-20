using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models
{
    public class TrainingProgram
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int MaxAttendees { get; set; }

        public Employee Employee { get; set; }

        public List<Employee>? Employees { get; set; } = new List<Employee>();
    }
}
