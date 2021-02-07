using Rental.Domain.Extensions;
using System;

namespace Rental.Domain.Entities
{
    public class Model : BaseEntity
    {
        public string Name { get; set; }

        public static Guid GetIdentifier(string name)
        {
            return new object[] { name }.GetIdentifier();
        }
    }
}
