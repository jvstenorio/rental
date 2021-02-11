using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Web.Models
{
    public class AddressDto
    {
        [Display(Name = "CEP")]
        public string ZipCode { get; set; }
        [Display(Name = "Rua")]
        public string Street { get; set; }
        [Display(Name = "Número")]
        public string Number { get; set; }
        [Display(Name = "Complemento")]
        public string Complement { get; set; }
        [Display(Name = "Cidade")]
        public string City { get; set; }
        [Display(Name = "Estado")]
        public string State { get; set; }
    }
}
