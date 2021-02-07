using Rental.Domain.Extensions;
using Rental.Domain.ValueObjects;
using System;

namespace Rental.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string Cpf { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }

        public static Guid GetIdentifier(string cpf)
        {
            return new object[] { cpf }.GetIdentifier();
        }
    }
}
