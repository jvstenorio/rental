using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
