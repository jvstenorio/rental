using System.Collections.Generic;

namespace Rental.Web.Models
{
    public class VehiclesViewModel
    {
        public VehiclesViewModel()
        {
            Categories = new List<CategoryDto>();
        }

        public List<CategoryDto> Categories { get; set; }
    }
}
