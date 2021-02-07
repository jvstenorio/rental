using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rental.Domain.Models
{
    public class BookingDto
    {
        public int? TotalHours { get; set; }
        public double? Price { get; set; }
        public VehicleDto Vehicle { get; set; }
        public CustomerDto Customer { get; set; }
    }
}
