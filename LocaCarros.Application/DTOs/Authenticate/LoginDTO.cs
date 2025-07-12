using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.Authenticate
{
    public class LoginDTO
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, ErrorMessage ="The {0} must be at least {2} and at max " +
            "{1} caracters long.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
