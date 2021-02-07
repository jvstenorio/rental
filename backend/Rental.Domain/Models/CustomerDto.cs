using Rental.Domain.ValueObjects;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rental.Domain.Models
{
    public class CustomerDto : UserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Cpf { get; set; }
        [Required]
        public DateTime? BirthDate { get; set; }
        [Required]
        public Address Address { get; set; }
    }
}
