using System;
namespace UserAvatar.API.Contracts.Requests
{
    public class CreateCardRequest
    {
        public int ColumnId { get; set; }
        public string Title { get; set; }
    }
}
