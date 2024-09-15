using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos;

/// <summary>
///     Data transfer
/// </summary>
public sealed class CommentDto
{
    /// <summary>
    ///     Comment body (Comment text)
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(256, MinimumLength = 1)]
    public string Text { get; set; }
}
