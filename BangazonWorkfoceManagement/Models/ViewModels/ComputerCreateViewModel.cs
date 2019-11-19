using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class ComputerCreateViewModel
    {
        public Computer Computer { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();

        public List<SelectListItem> EmployeeOptions
        {
            get
            {
                if (Employees == null) return null;

                return Employees
                    .Select(e => new SelectListItem(e.FullName, e.Id.ToString()))
                    .ToList();
            }
        }
    }
}
