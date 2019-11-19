using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class AssignTrainingViewModel
    {
        public TrainingProgram TrainingProgram { get; set; }
        public Employee Employee { get; set; }
        public List<TrainingProgram> AllTrainingPrograms { get; set; } = new List<TrainingProgram>();

        [Display(Name = "Upcoming Trainings")]
        public List<TrainingProgram> FutureTrainings
        {
            get
            {
                return AllTrainingPrograms.Where(tp => tp.StartDate > DateTime.Now).ToList();
            }
        }
        public List<SelectListItem> TrainingOptions
        {
            get
            {
                if (FutureTrainings == null) return null;
                return FutureTrainings
                    .Select(ft => new SelectListItem(ft.Name, ft.Id.ToString()))
                    .ToList();
            }
        }
    }
}
