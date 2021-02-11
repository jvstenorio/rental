using System.Collections.Generic;

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
