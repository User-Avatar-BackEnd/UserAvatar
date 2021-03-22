using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class UserShortDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Rank { get; set; }
    }
}
