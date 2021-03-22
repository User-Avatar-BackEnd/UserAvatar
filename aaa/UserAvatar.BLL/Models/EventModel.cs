using System;
using System.Collections.Generic;

namespace UserAvatar.BLL.Models
{
    public class EventModel
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public List<HistoryModel> Histories { get; set; }
    }
}
