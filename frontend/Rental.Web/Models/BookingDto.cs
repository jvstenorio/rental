using System;
using System.ComponentModel.DataAnnotations;

namespace Rental.Web.Models
{
    public class BookingDto
    {
        public string BookingCode { get; set; }
        [Display(Name = "Total de Horas")]
        [Required]
        public int TotalHours { get; set; }
        [Display(Name = "Preço")]
        public double? Price { get; set; }
        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        [Display(Name = "Placa")]
        [Required]
        public string Plate { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
