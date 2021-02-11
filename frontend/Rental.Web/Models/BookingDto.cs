using System;
using System.ComponentModel.DataAnnotations;

namespace Rental.Web.Models
{
    public class BookingDto
    {
        public string BookingCode { get; set; }

        [Display(Name = "Total de Horas")]
        [Required(ErrorMessage = "O total de horas deve ser inserido!")]
        public int TotalHours { get; set; }

        [Display(Name = "Preço")]
        public double? Price { get; set; }

        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        [Display(Name = "Placa")]
        [Required(ErrorMessage = "A placa deve ser inserida!k")]
        public string Plate { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
