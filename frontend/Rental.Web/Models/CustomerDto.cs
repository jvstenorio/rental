using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Web.Models
{
    public class CustomerDto : UserDto
    {
        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Nome obirgatório")]
        public new string Name { get; set; }
        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O CPF deve ser inserido")]
        public string Cpf { get; set; }
        [Display(Name = "Data de Nascimento")]
        [Required(ErrorMessage = "A data de nascimento deve ser inserida")]
        public DateTime BirthDate { get; set; }
        [Required]
        public AddressDto Address { get; set; }
    }
}
