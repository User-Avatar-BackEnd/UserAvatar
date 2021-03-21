using System;
using System.Collections.Generic;
using UserAvatar.Bll.Models;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class TaskDetailedDto
    {
        public int Id { get; set; }
        public int ColumnId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ResponsibleId { get; set; }
        public int Priority { get; set; }
        public bool IsHidden { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
