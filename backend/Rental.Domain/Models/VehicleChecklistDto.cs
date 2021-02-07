namespace Rental.Domain.Models
{
    public class VehicleChecklistDto
    {
        public bool FullFuelTank { get; set; }
        public bool Clean { get; set; }
        public bool Smashed { get; set; }
        public bool Scratches { get; set; }
    }
}
