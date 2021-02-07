using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Web.Models
{
    public class CategoryDto
    {
        public CategoryDto()
        {
            Vehicles = new List<VehicleDto>();
        }

        public List<VehicleDto> Vehicles { get; set; }
        public string Name { get; set; }
    }
}
