using System;
namespace UserAvatar.Api.Contracts.Requests
{
    public class CreateCardRequest
    {
        public int ColumnId { get; set; }
        public string Title { get; set; }
    }
}
