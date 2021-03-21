using System;
namespace UserAvatar.Api.Contracts.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
