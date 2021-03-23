using System;
namespace UserAvatar.Bll.TaskManager.Models
{
    public class InviteModel
    {
        public int Id { get; set; }
        public int InviterId { get; set; }
        public int InvitedId { get; set; }
        public int BoardId { get; set; }
        public int Status { get; set; }
        public DateTime Issued { get; set; }
    }
}
