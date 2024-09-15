using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos;

/// <summary>
///     Data transfer object of event
/// </summary>
public sealed class EventDto
{
    /// <summary>
    ///     Event name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; }

    /// <summary>
    ///     Score for event
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Score { get; set; }
}
