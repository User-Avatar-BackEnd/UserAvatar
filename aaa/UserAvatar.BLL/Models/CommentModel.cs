using System;
namespace UserAvatar.BLL.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public CardModel Card { get; set; }
        public UserModel User { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
