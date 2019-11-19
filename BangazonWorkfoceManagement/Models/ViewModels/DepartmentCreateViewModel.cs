using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models.ViewModels
{
    public class DepartmentCreateViewModel
    {
        public Department Department { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}