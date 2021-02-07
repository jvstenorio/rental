namespace Rental.Domain.ValueObjects
{
    public class Address
    {
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public Address() { }
        public Address(string zipCode, string street, string number, string complement, string city, string state)
        {
            ZipCode = zipCode;
            Street = street;
            Number = number;
            Complement = complement;
            City = city;
            State = state;
        }
    }
}
