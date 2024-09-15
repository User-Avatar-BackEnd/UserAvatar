namespace UserAvatar.Api.Contracts.ViewModels;

/// <summary>
///     Daily Event ViewModel (Killer feature)
/// </summary>
public sealed class DailyEventVm
{
    /// <summary>
    ///     Event name
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    ///     Flag if this event is completed
    /// </summary>
    public bool IsCompleted { get; set; }
}
