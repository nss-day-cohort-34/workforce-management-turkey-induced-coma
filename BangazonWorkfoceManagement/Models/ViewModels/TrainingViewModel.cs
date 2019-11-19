using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class TrainingViewModel
    {
        public TrainingProgram TrainingProgram { get; set; }
        public int TrainingProgramId { get; set; }
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

        [Display(Name = "Past Trainings")]
        public List<TrainingProgram> PastTrainings
        {
            get
            {
                return AllTrainingPrograms.Where(tp => tp.StartDate <= DateTime.Now).ToList();
            }
        }
        public List<SelectListItem> TrainingOptions
        {
            get
            {
                if (FutureTrainings == null) return null;
                return FutureTrainings
                    .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                    .ToList();
            }
        }
    }
}
