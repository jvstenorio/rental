using System.ComponentModel.DataAnnotations;

namespace Rental.Domain.Models
{
    public class MakeDto
    {
        [Required]
        public string Name { get; set; }
    }
}
