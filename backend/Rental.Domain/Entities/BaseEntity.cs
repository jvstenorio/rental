using System;

namespace Rental.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Identifier { get; set; }
    }
}
