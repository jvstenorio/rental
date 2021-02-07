namespace Rental.Domain.Models
{
    public class EmployeeDto : UserDto
    {
        public string RegistrationNumber { get; set; }
        public string Name { get; set; }
    }
}
