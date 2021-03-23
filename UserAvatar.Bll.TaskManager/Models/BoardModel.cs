using System;
using System.Collections.Generic;

namespace UserAvatar.Bll.TaskManager.Models
{
    public class BoardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public List<ColumnModel> Columns { get; set; }
        public List<MemberModel> Members { get; set; }
                
        public int ModifiedBy { get; set; }
    }
}
