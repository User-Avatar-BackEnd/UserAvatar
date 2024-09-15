namespace UserAvatar.Api.Contracts.ViewModels;

/// <summary>
///     Event viewModel
/// </summary>
public sealed class EventVm
{
    /// <summary>
    ///     Event name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Score of this event
    /// </summary>
    public int Score { get; set; }
}
