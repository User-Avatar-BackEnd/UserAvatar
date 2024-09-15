namespace UserAvatar.Api.Authentication;

/// <summary>
///     Interface for ApplicationUser class
/// </summary>
public interface IApplicationUser
{
    /// <summary>
    ///     User request id from token
    /// </summary>
    int Id { get; }
}
