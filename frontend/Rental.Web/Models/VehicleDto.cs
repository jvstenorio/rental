using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Web.Models
{
    public class VehicleDto
    {
        [Required]
        public string Plate { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int? Year { get; set; }
        [Required]
        public double? PricePerHour { get; set; }
        [Required]
        public string Fuel { get; set; }
        [Required]
        public int? TrunkSize { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
