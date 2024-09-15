using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests;

/// <summary>
///     Request for password change
/// </summary>
public sealed class ChangePasswordRequest
{
    /// <summary>
    ///     Old user password
    /// </summary>
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid old password")]
    public string OldPassword { get; set; }

    /// <summary>
    ///     New user password
    /// </summary>
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid new password")]
    public string NewPassword { get; set; }
}
