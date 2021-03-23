using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class CommentDto
    {
        [Required]
        //todo: add validator
        public string Text { get; set; }
    }
}