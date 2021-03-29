using System.Collections.Generic;

namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Detailed viewmodel of card
    /// </summary>
    public class CardDetailedVm
    {
        /// <summary>
        /// Card id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Column id where this card is
        /// </summary>
        public int ColumnId { get; set; }
        /// <summary>
        /// Card title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Card description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Id of this card responsible user
        /// </summary>
        public int? ResponsibleId { get; set; }
        /// <summary>
        /// This card priority
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Flag if this card is hidden
        /// </summary>
        public bool IsHidden { get; set; }
        /// <summary>
        /// List of this card comments
        /// </summary>
        public List<CommentVm> Comments { get; set; }
    }
}
