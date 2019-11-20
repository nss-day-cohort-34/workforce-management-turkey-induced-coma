using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class ComputerCreateViewModel
    {
        public Computer Computer { get; set; }
        public Employee Employee { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        [Display (Name = "Assign Employee")]
        public List<SelectListItem> EmployeeOptions
        {
            get
            {
                if (Employees == null) return null;

                var listItems = Employees
                    .Select(e => new SelectListItem(e.FullName, e.Id.ToString()))
                    .ToList();
                listItems.Insert(0, new SelectListItem
                {
                    Text = "Choose an employee...",
                    Value = "0"
                });


                return listItems;
            }
        }
    }
}
