using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkfoceManagement.Models
{
    public class Computer
    {
        public int Id { get; set; }
        [Required]
        [Display (Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [Display (Name = "Decomission Date")]
        public DateTime DecomissionDate { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Manufacturer { get; set; }
    }
}
