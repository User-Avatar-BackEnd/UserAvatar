namespace UserAvatar.Api.Contracts.ViewModels;

/// <summary>
///     User short ViewModel
/// </summary>
public sealed class UserShortVm
{
    /// <summary>
    ///     User id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     User login
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    ///     User rank
    /// </summary>
    public string Rank { get; set; }
}
