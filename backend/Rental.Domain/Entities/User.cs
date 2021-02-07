using Rental.Domain.Enumerations;
using Rental.Domain.Extensions;
using System;

namespace Rental.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Profile Profile { get; set; }
        public static Guid GetIdentifier(string login)
        {
            return new object[] { login }.GetIdentifier();
        }
    }
}
