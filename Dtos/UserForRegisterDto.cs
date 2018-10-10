using System;
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
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public DateTime DateOfBirth {get; set;}
        [Required]
        public string Country {get; set;}
        public string gender { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; }



        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;

        }
        
    }
}