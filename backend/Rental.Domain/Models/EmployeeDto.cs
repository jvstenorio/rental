﻿using System.ComponentModel.DataAnnotations;

namespace Rental.Domain.Models
{
    public class EmployeeDto : UserDto
    {
        [Required]
        public string RegistrationNumber { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
