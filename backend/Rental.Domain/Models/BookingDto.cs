namespace Rental.Domain.Models
{
    public class BookingDto
    {
        public string BookingCode { get; set; }
        public int? TotalHours { get; set; }
        public double? Price { get; set; }
        public string Cpf { get; set; }
        public string Plate { get; set; }
    }
}
