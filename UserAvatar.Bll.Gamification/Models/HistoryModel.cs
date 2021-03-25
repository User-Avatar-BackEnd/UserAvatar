using System;
namespace UserAvatar.Bll.Gamification.Models
{
    public class HistoryModel
    {
        public string EventName { get; set; }

        public int Sore { get; set; }

        public DateTimeOffset DateTime { get; set; }
    }
}
