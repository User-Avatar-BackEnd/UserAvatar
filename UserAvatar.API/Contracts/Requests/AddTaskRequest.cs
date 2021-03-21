using System;
namespace UserAvatar.API.Contracts.Requests
{
    public class AddTaskRequest
    {
        public int ColumnId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ResponsibleId { get; set; }
        public int Priority { get; set; }
    }
}
