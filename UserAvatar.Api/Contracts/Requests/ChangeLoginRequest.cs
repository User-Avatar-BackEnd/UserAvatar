using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests
{
    public class ChangeLoginRequest
    {
        [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid login")]
        public string Login { get; set; }
    }
}