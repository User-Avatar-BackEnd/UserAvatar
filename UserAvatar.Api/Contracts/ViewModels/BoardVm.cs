using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAvatar.Api.Contracts.ViewModels
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
