using System.ComponentModel.DataAnnotations;

namespace Rental.Domain.Models
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
