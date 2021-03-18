using System.ComponentModel.DataAnnotations;

namespace UserAvatar.API.Contracts
{
    public class AuthRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 6)]
        public string Login { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 6)]
        public string Password { get; set; }
    }
}