using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class TrainingDetailViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Program")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }
        [Display(Name = "Employee Count")]
        public int? EmployeeCount { get; set; }
    }
}
