using System;
namespace UserAvatar.Api.Contracts.Requests
{
    public class CreateCardDto
    {
        public int ColumnId { get; set; }
        public string Title { get; set; }
    }
}
