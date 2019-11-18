using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public List<Department> Departments { get; set; } = new List<Department>();
        public List<SelectListItem> DepartmentOptions
        {
            get
            {
                if (Departments == null) return null;
                return Departments
                    .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                    .ToList();
            }
        }
        public Employee Employee { get; set; }
        public List<SelectListItem> ComputerOptions
        {
            get
            {
                if (UnassignedComputers == null) return null;
                var listItems = UnassignedComputers
                        .Select(c => new SelectListItem($"{c.Manufacturer} {c.Make}", c.Id.ToString()))
                        .ToList();
                listItems.Insert(0, new SelectListItem
                {
                    Text = "Choose a computer...",
                    Value = "0"
                });

                return listItems;
            }
        }
        public List<Computer> UnassignedComputers { get; set; } = new List<Computer>();
        public Computer CurrentComputer { get; set; }
    }
}
