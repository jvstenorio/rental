using System.ComponentModel.DataAnnotations;

namespace Rental.Web.Models
{
    public class UserDto
    {
        [Required(ErrorMessage = "O login deve ser inserido.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A senha deve ser inserida.")]
        public string Password { get; set; }

        public int Profile { get; set; }
        public string Name { get; set; }
    }
}
