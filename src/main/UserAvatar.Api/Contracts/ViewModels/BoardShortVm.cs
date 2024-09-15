namespace UserAvatar.Api.Contracts.ViewModels;

/// <summary>
///     Short board viewmodel
/// </summary>
public sealed class BoardShortVm
{
    /// <summary>
    ///     Board id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Board title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Flag is this user is owner of this board
    /// </summary>
    public bool IsOwner { get; set; }
}
