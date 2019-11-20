using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models
{
    public class Employee
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Surname")]
        public string LastName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        [Required]
        [Display(Name = "Supervisor")]
        public Boolean IsSupervisor { get; set; }
        public Computer AssignedComputer { get; set; }
        [Display(Name = "Available Computers")]
        public int ComputerId { get; set; }
        public List<TrainingProgram>? AllTrainingPrograms { get; set; } = new List<TrainingProgram>();


        [Display(Name = "Upcoming Trainings")]
        public List<TrainingProgram> FutureTrainings
        {
            get
            {
                return AllTrainingPrograms.Where(tp => tp.StartDate > DateTime.Now).ToList();
            }
        }
        [Display(Name = "Past Trainings")]
        public List<TrainingProgram> PastTrainings
        {
            get
            {
                return AllTrainingPrograms.Where(tp => tp.StartDate <= DateTime.Now).ToList();
            }
        }

    }
}