using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Dtos;

namespace UserAvatar.Api.Contracts.ViewModel
{
    public class BoardVm
    {
        public int Id { get; set; }

        public bool IsOwner { get; set; }

        public string Title { get; set; }

        public List<UserShortVm> Members { get; set; }

        public List<FullColumnVm> Columns { get; set; }
    }
}
