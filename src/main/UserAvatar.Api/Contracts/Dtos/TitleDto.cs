using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos;

/// <summary>
///     Data transfer object of title
/// </summary>
public sealed class TitleDto
{
    /// <summary>
    ///     Title body
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]
    public string Title { get; set; }
}
