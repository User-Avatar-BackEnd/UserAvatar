using System;
using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class TitleDto
    {
        [Required(AllowEmptyStrings=false)]
        [StringLength(64, MinimumLength = 1)]
        public string Title { get; set; }
    }
}
