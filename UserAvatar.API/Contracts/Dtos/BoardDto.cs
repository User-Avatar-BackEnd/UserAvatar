using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Dtos;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class BoardDto
    {
        public int Id { get; set; }

        public bool IsOwner { get; set; }

        public string Title { get; set; }

        public List<UserShortDto> Members { get; set; }

        public List<FullColumnDto> Columns { get; set; }
    }
}
