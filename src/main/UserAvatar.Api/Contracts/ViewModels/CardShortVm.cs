namespace UserAvatar.Api.Contracts.ViewModels;

/// <summary>
///     Short viewmodel of card
/// </summary>
public sealed class CardShortVm
{
    /// <summary>
    ///     Card id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Title of this card
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Card description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Priority of card
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    ///     Id of responsible user
    /// </summary>
    public int? ResponsibleId { get; set; }

    /// <summary>
    ///     Flag if this card is hidden
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    ///     If of column where this card is situated
    /// </summary>
    public int ColumnId { get; set; }

    /// <summary>
    ///     Amount of comments in this card
    /// </summary>
    public int CommentsCount { get; set; }
}
