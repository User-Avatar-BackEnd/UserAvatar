using System;
using System.Collections.Generic;

namespace UserAvatar.Bll.TaskManager.Models
{
    public class BoardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OwnerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public List<ColumnModel> Columns { get; set; }
        public List<MemberModel> Members { get; set; }
                
        public int ModifiedBy { get; set; }
    }
}
