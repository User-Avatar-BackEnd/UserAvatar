using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid old password")]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid new password")]
        public string NewPassword { get; set; }
    }
}