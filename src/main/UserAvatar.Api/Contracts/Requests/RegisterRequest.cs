using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests;

/// <summary>
///     Request for registration
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    ///     User email
    /// </summary>
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    /// <summary>
    ///     User login
    /// </summary>
    [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$|^\s*$", ErrorMessage = "Invalid login")]
    public string Login { get; set; }

    /// <summary>
    ///     User password
    /// </summary>
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid password")]
    public string Password { get; set; }
}
