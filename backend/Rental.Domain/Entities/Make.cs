using Rental.Domain.Extensions;
using System;

namespace Rental.Domain.Entities
{
    public class Make : BaseEntity
    {
        public string Name { get; set; }

        public static Guid GetIdentifier(string name)
        {
            return new object[] { name }.GetIdentifier();
        }
    }
}
