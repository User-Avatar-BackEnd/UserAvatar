using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests
{
    public class LoginRequest
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
