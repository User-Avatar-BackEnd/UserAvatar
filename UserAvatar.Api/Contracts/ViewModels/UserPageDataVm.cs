using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAvatar.Api.Contracts.ViewModels
{
    public class UserPageDataVm
    {
        public int Position { get; set; }

        public string Rank { get; set; }

        public string Login { get; set; }

        public int Score { get; set; }

        public string Role { get; set; }
    }
}
