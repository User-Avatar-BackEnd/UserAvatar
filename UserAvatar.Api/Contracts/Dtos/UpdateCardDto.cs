using System;
using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos
{
    /// <summary>
    /// Data transfer object of card
    /// </summary>
    public class UpdateCardDto
    {
        /// <summary>
        /// Id of card
        /// </summary>
        [Required]
        public int ColumnId { get; set; }

        /// <summary>
        /// Title of card
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 1)]
        public string Title { get; set; }

        /// <summary>
        /// Description of card
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Id of responsible user
        /// </summary>
        public int? ResponsibleId { get; set; }
        /// <summary>
        /// Priority level
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// Is this card is hidden
        /// </summary>
        public bool IsHidden { get; set; }
    }
}