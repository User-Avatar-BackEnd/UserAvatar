namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// ViewModel of card
    /// </summary>
    public class CardVm
    {
        /// <summary>
        /// Card id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Title of card
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Card description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Priority of card
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Id of responsible user
        /// </summary>
        public int? ResponsibleId { get; set; }
        /// <summary>
        /// Flag if this card is hidden
        /// </summary>
        public bool IsHidden { get; set; }
        /// <summary>
        /// Count of comments in this card
        /// </summary>
        public int CommentsCount { get; set; }
    }
}