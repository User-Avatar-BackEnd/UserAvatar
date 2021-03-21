using System;
using System.Collections.Generic;

namespace UserAvatar.Bll.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public ColumnModel Column { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UserModel Owner { get; set; }
        public UserModel Responsible { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int Priority { get; set; }
        public bool IsHidden { get; set; }
        public List<CommentModel> Comments { get; set; }
    }
}
