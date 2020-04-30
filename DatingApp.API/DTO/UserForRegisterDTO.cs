using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Deve haver entre 4 e 8 caracteres")]
        public string Password { get; set; }
    }
}