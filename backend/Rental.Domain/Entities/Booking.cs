using Rental.Domain.Enumerations;
using Rental.Domain.Extensions;
using System;

namespace Rental.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string BookingCode { get; set; }
        public string Plate { get; set; }
        public string Cpf { get; set; }
        public int TotalHours { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public DateTimeOffset Date { get; set; }

        public static Guid GetIdentifier(string bookingCode)
        {
            return new object[] { bookingCode }.GetIdentifier();
        }
    }
}
