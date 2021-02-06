using System;

namespace Rental.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Indentifier { get; set; }
    }
}
