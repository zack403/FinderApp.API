using System.ComponentModel.DataAnnotations;

namespace FinderApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Please provide a password not less than 4 and not greater than 8")]
        public string Password { get; set; }
    }
}