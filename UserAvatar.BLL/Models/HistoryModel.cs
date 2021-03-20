using System;
namespace UserAvatar.BLL.Models
{
    public class HistoryModel
    {
        public int Id { get; set; }
        public UserModel User { get; set; }
        public EventModel Event { get; set; }
        public DateTime DateTime { get; set; }
        public bool Calculated { get; set; }
    }
}
