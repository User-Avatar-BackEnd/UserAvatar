using System.ComponentModel.DataAnnotations;

namespace UserAvatar.API.Contracts.Requests
{
    public class ChangePasswordRequest
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 5)]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 5)]
        public string NewPassword { get; set; }
    }
}