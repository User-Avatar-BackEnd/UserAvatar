using System;
namespace UserAvatar.Bll.TaskManager.Models
{
    public class HistoryModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EventName { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public bool Calculated { get; set; }
    }
}
