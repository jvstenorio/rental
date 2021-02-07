using Rental.Domain.Extensions;
using System;

namespace Rental.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public string Plate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public double? PricePerHour { get; set; }
        public string Fuel { get; set; }
        public int? TrunkSize { get; set; }
        public string Category { get; set; }

        public static Guid GetIdentifier(string Plate)
        {
            return new object[] { Plate }.GetIdentifier();
        }
    }
}
