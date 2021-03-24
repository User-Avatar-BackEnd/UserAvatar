using System;
namespace UserAvatar.Bll.TaskManager.Models
{
    public class InviteModel
    {
        public int Id { get; set; }
        public int InvitedId { get; set; }
        public UserModel Inviter { get; set; }
        
        public BoardModel Board { get; set; }
        public int Status { get; set; }
        public DateTimeOffset Issued { get; set; }
    }
}
