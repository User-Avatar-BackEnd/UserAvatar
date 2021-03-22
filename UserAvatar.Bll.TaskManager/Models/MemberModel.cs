using System;
namespace UserAvatar.Bll.TaskManager.Models
{
    public class MemberModel
    {
        public int Id { get; set; }
        public UserModel User { get; set; }
        public BoardModel Board { get; set; }
    }
}
