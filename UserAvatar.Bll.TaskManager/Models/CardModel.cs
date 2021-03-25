using System;
using System.Collections.Generic;

namespace UserAvatar.Bll.TaskManager.Models
{
    public class CardModel
    {
        public int Id { get; set; }
        public int ColumnId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public int? ResponsibleId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public int? Priority { get; set; }
        public bool IsHidden { get; set; }
        public List<CommentModel> Comments { get; set; }
                
        public int ModifiedBy { get; set; }
    }
}
