using System;
using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class EventDto
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128, MinimumLength = 1)]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int Score { get; set; }
    }
}
