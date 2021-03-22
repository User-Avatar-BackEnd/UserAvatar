using System;
namespace UserAvatar.Api.Contracts.Requests
{
    public class UpdateCardRequest
    {
        public int Id { get; set; }
        public int ColumnId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ResponsibleId { get; set; }
        public int? Priority { get; set; }
        public bool IsHidden { get; set; }
    }
}
