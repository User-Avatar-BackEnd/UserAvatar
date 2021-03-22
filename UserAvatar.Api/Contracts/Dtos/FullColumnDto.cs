using System.Collections.Generic;
using UserAvatar.Api.Contracts.Dtos;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class FullColumnDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public List<CardDto> Cards { get; set; }
    }
}