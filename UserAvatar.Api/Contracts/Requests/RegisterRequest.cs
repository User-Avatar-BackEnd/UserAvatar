using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$|^\s*$", ErrorMessage ="Invalid login")]
        public string Login { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; }
    }
}