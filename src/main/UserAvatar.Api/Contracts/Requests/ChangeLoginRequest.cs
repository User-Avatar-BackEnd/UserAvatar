using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests;

/// <summary>
///     Change login request
/// </summary>
public sealed class ChangeLoginRequest
{
    /// <summary>
    ///     New user login
    /// </summary>
    [RegularExpression(@"^[a-zA-Z0-9_.-]{5,}$", ErrorMessage = "Invalid login")]
    public string Login { get; set; }
}
