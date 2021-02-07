using System.ComponentModel.DataAnnotations;

namespace Rental.Domain.Models
{
    public class ModelDto
    {
        [Required]
        public string Name { get; set; }
    }
}
