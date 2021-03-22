using System;
namespace UserAvatar.Api.Contracts.Requests
{
    public class AddTaskRequest
    {
        public int ColumnId { get; set; }
        public string Title { get; set; }
    }
}
