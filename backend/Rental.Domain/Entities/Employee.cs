using Rental.Domain.Extensions;
using System;

namespace Rental.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string RegistrationNumber { get; set; }
        public string Name { get; set; }

        public static Guid GetIdentifier(string registrationNumber)
        {
            return new object[] { registrationNumber }.GetIdentifier();
        }
    }
}
