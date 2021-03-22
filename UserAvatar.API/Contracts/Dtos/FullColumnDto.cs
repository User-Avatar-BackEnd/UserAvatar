using System.Collections.Generic;
using UserAvatar.API.Contracts.Dtos;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class FullColumnDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }
}