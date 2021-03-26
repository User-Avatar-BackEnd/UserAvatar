using System;
using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos
{
    //todo: validations
    public class UpdateCardDto
    {
        [Required]
        public int ColumnId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 1)]
        public string Title { get; set; }

        public string Description { get; set; }
        public int? ResponsibleId { get; set; }
        public int? Priority { get; set; }
        public bool IsHidden { get; set; }
    }
}