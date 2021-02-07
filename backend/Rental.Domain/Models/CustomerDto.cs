using Rental.Domain.ValueObjects;
using System;

namespace Rental.Domain.Models
{
    public class CustomerDto : UserDto
    {
        public string Name { get; set; }
        public string Cpf { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }
    }
}
