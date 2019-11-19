using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class PastTrainingDetailsViewModel
    {
        public TrainingProgram TrainingProgram { get; set; }
        public List<Employee> Attendees { get; set; } = new List<Employee>();
    }
}
