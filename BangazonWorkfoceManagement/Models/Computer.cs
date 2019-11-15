using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models
{
    public class Computer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        [Display(Name = "Assign Date")]
        public DateTime AssignDate { get; set; }
        public DateTime DecommissionDate { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Manufacturer { get; set; }
    }
}
